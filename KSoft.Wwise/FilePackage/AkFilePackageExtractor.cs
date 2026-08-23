using System;
using System.Collections.Generic;

namespace KSoft.Wwise.FilePackage
{
	public sealed class AkFilePackageExtractor
	{
		public string PackageFileName { get; private set; }
		public AkFilePackage Package { get; private set; }

		public IReadOnlyDictionary<uint, string> EventToSoundNameMap { get; private set; }

		public AkFilePackageExtractor(string packageFileName, AkFilePackage package,
			IReadOnlyDictionary<uint, string> eventToSoundNameMap)
		{
			ArgumentException.ThrowIfNullOrEmpty(packageFileName);
			ArgumentNullException.ThrowIfNull(package);
			ArgumentNullException.ThrowIfNull(eventToSoundNameMap);

			PackageFileName = packageFileName;
			Package = package;
			EventToSoundNameMap = eventToSoundNameMap;
		}

		internal readonly Dictionary<SoundBank.HircType, Dictionary<uint, SoundBank.AkSoundBankHierarchyObjectBase>> mObjects =
			new();

		internal readonly Dictionary<uint, SoundBank.AkSoundBankHierarchyObjectBase> mIdToObject =
			new();

		internal readonly HashSet<uint> mDupObjects = new();

		internal readonly Dictionary<uint, SoundBank.MediaReference> mUntouched =
			new();

		readonly Dictionary<uint, SoundBank.AkSoundBank> mIdToBank =
			new();

		bool mPreparedForExtraction;
		public void PrepareForExtraction()
		{
			if (mPreparedForExtraction)
			{
				return;
			}

			foreach (var bank in Package.SoundBanks)
			{
				mIdToBank.Add(bank.Id, bank);
				bank.CopyObjectsTo(this);
			}

			BuildSoundNames();

			foreach (var bank in Package.SoundBanks)
			{
				bank.PrepareForExtraction();
			}

			mPreparedForExtraction = true;
		}

		public void BuildSoundNames()
		{
			if (!mObjects.TryGetValue(SoundBank.HircType.Event, out var events))
			{
				Debug.Trace.FilePackage.TraceInformation("{0} - No events?",
					PackageFileName);
				return;
			}

			foreach (var kv in events)
			{
				var e = (kv.Value as SoundBank.AkSoundBankHierarchyEvent)!;

				if (!EventToSoundNameMap.TryGetValue(e.ID, out var eventName))
				{
					continue;
				}
				e.Name = eventName;

				foreach (var action_id in e.ActionList)
				{
					var action = (mIdToObject[action_id] as SoundBank.AkSoundBankHierarchyAction)!;
					if (action.Type != SoundBank.AkActionType.Play)
					{
						continue;
					}

					SoundBank.AkSoundBankHierarchyObjectBase target = mIdToObject[action.TargetID];
					switch (target)
					{
						case SoundBank.AkSoundBankHierarchySound sound:
							sound.Name = eventName.Replace("play_", "");
							break;
						case SoundBank.AkSoundBankHierarchyRanSeqCntr ranSeq:
						{
							if (ranSeq.Playlist != null)
							{
								foreach (var item in ranSeq.Playlist)
								{
									if (mIdToObject.TryGetValue(item.ID, out var itemObj) &&
										itemObj is SoundBank.AkSoundBankHierarchySound itemSound)
									{
										itemSound.Name = eventName.Replace("play_", "") + "_" + item.ID.ToString("X8");
									}
									else
									{
										Debug.Trace.FilePackage.TraceInformation("{0} - {1}: couldn't name item {2} {3}",
											PackageFileName, eventName, item.ID.ToString("X8"), item.GetType().Name);
									}
								}
							}
							else
							{
								Debug.Trace.FilePackage.TraceInformation("{0} - {1}: couldn't name playlist {2} {3}",
									PackageFileName, eventName, target.ID.ToString("X8"), SoundBank.HircType.RanSeqCntr.ToString());
							}

							break;
						}

						default:
							Debug.Trace.FilePackage.TraceInformation("{0} - {1}: couldn't name {2} {3}",
								PackageFileName, eventName, target.ID.ToString("X8"), target.ToString());
							break;
					}
				}
			}
		}

		public void ExtractSounds(string path, System.IO.StreamWriter towav, IO.EndianReader pckReader,
			bool overwriteExisting = false)
		{
			if (!mObjects.TryGetValue(SoundBank.HircType.Sound, out var sounds))
			{
				Debug.Trace.FilePackage.TraceInformation("{0} - No sounds to extract?",
					PackageFileName);
				return;
			}

			foreach (var kv in sounds)
			{
				var snd = (kv.Value as SoundBank.AkSoundBankHierarchySound)!;
				if (snd.Name == null && mDupObjects.Contains(kv.Key))
				{
					continue;
				}

				uint src_id = snd.Source.MediaInfo.SourceID;
				mUntouched.Remove(src_id);

				if (snd.Source.MediaInfo.FileID == 0)
				{
					towav.WriteLine("REM NoData {0}", snd.Name);
					continue;
				}

				string dir;
				string filename = (snd.Name ?? ("unknown_" + kv.Key.ToString("X8"))) + ".xma";

				uint bank_id = snd.BankId;
				if (!Package.IdToName.TryGetValue(bank_id, out string? bank_name))
				{
					bank_name = bank_id.ToString("X8");
				}

				SoundBank.AkSoundBankData? bank_data = null;
				bool streamed = snd.Source.StreamType != SoundBank.AkBankSourceData.SourceType.Data;
				if (!streamed)
				{
					bank_data = mIdToBank[bank_id].mData;
				}

				dir = System.IO.Path.Combine(path, bank_name!);
				System.IO.Directory.CreateDirectory(dir);

				string full_path = System.IO.Path.Combine(dir, filename);
				towav.WriteLine("towav.exe {0}", full_path);

				if (System.IO.File.Exists(full_path) && !overwriteExisting)
				{
					continue;
				}

				using (var fs = System.IO.File.Create(full_path))
				{
					if (streamed)
					{
						var file = Package.FindStreamedFileById(snd.Source.MediaInfo.FileID);
						pckReader.Seek(file.FileOffset);
						byte[] data = pckReader.ReadBytes((int)file.FileSize32);

						fs.Write(data, 0, data.Length);
					}
					else
					{
						fs.Write(bank_data!.Buffer, (int)snd.Source.MediaInfo.FileOffset, (int)snd.Source.MediaInfo.MediaSize);
					}
				}
			}
			//////////////////////////////////////////////////////////////////////////
			foreach (var kv in mUntouched)
			{
				var mr = kv.Value;

				string name = "unknown2_" + mr.Media.ID.ToString("X8");
				if (mr.Media.Size == 0)
				{
					towav.WriteLine("REM NoData2 {0}", name);
					continue;
				}

				string filename = name + ".xma";

				uint bank_id = mr.BankId;
				if (!Package.IdToName.TryGetValue(bank_id, out string? bank_name))
				{
					bank_name = bank_id.ToString("X8");
				}

				SoundBank.AkSoundBankData? bank_data = mIdToBank[bank_id].mData;

				string dir = System.IO.Path.Combine(path, bank_name!);
				System.IO.Directory.CreateDirectory(dir);

				string full_path = System.IO.Path.Combine(dir, filename);
				towav.WriteLine("towav.exe {0}", full_path);

				if (System.IO.File.Exists(full_path))
				{
					continue;
				}

				using (var fs = System.IO.File.Create(full_path))
				{
					fs.Write(bank_data!.Buffer, (int)mr.Media.Offset, (int)mr.Media.Size);
				}
			}
		}
	};
}
