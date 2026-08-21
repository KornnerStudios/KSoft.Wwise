using System.Collections.Generic;

namespace KSoft.Wwise.SoundBank
{
	using HircTypeStreamer8 = IO.EnumBinaryStreamer<HircType, byte>;
	using HircTypeStreamer32 = IO.EnumBinaryStreamer<HircType, uint>;

	partial class AkSoundBankObjectBase
	{
		static readonly Values.GroupTagData32 kHierarchySignature = new(
			"HIRC", "audiokinetic_hierarchy"); // BankHierarchyChunkID

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
		static AkSoundBankObjectBase NewHIRC(uint generatorVersion)
		{
			return generatorVersion switch
			{
				_ => new AkSoundBankHierarchy(),
			};
		}
	};

	sealed class AkSoundBankHierarchy
		: AkSoundBankObjectBase
	{
		struct AKBKSubHircSection
			: IO.IEndianStreamSerializable
		{
			public HircType Type;
			public uint SectionSize;

			#region IEndianStreamSerializable Members
			public void Serialize(IO.EndianStream s)
			{
				uint sdk_ver = (KSoft.Debug.TypeCheck.CastReference<AkSoundBank>(s.Owner)).SdkVersion;

				s.Stream(ref Type, AkVersion.HircTypeIs8bit(sdk_ver)
					? HircTypeStreamer8.Instance
					: HircTypeStreamer32.Instance);
				s.Stream(ref SectionSize);
			}
			#endregion
		};

		readonly Dictionary<HircType, Dictionary<uint, AkSoundBankHierarchyObjectBase>> mObjects = new();
		readonly Dictionary<uint, AkSoundBankHierarchyObjectBase> mIdToObject = new();

		public void CopyObjectsTo(FilePackage.AkFilePackageExtractor extractor)
		{
			foreach (var kv in mObjects)
			{
				var type = kv.Key;

				if (type == HircType.Attenuation)
				{
					continue;
				}

				if (!extractor.mObjects.TryGetValue(type,
						out Dictionary<uint, AkSoundBankHierarchyObjectBase> dic))
				{
					extractor.mObjects.Add(type, dic = new Dictionary<uint, AkSoundBankHierarchyObjectBase>());
				}

				foreach (var obj in kv.Value)
				{
					if (dic.ContainsKey(obj.Key))
					{
						extractor.mDupObjects.Add(obj.Key);
						continue;
					}

					dic.Add(obj.Key, obj.Value);
					extractor.mIdToObject.Add(obj.Key, obj.Value);
				}
			}
		}

		void MapObject(HircType type, AkSoundBankHierarchyObjectBase obj)
		{
			if (!mObjects.TryGetValue(type,
					out Dictionary<uint, AkSoundBankHierarchyObjectBase> dic))
			{
				mObjects.Add(type, dic = new Dictionary<uint, AkSoundBankHierarchyObjectBase>());
			}

			dic.Add(obj.ID, obj);
			mIdToObject.Add(obj.ID, obj);
		}

		#region IEndianStreamSerializable Members
		void SerializeItem(IO.EndianStream s, AKBKSubHircSection section)
		{
			if (!s.IsReading)
			{
				throw new System.InvalidOperationException(string.Format(
					"Hierarchy item serialization requires a readable stream; stream mode is {0}.",
					s.StreamMode));
			}

			using (s.EnterVirtualBufferWithBookmark(section.SectionSize))
			{
				var obj = AkSoundBankHierarchyObjectBase.New(section.Type);
				if (obj != null)
				{
					s.Stream(obj);

					MapObject(section.Type, obj);
				}
			}
		}
		void FromStream(IO.EndianStream s, AkSubchunkHeader header)
		{
			var bank = KSoft.Debug.TypeCheck.CastReference<AkSoundBank>(s.Owner);
			Util.MarkUnusedVariable(ref bank);

			using (s.EnterVirtualBufferWithBookmark(header.ChunkSize))
			{
				for (int x = 0, num_hirc_items = s.Reader.ReadInt32(); x < num_hirc_items; x++)
				{
					var section = new AKBKSubHircSection();
					section.Serialize(s);

					SerializeItem(s, section);
				}
			}
		}
		public override void Serialize(IO.EndianStream s, AkSubchunkHeader header)
		{
			if (s.IsReading)
			{
				FromStream(s, header);
			}
		}
		#endregion

		internal void PrepareForExtraction(AkSoundBank bank)
		{
			foreach (var kv in mObjects)
			{
				if (kv.Key != HircType.Sound)
				{
					continue;
				}

				foreach (var dic in kv.Value)
				{
					KSoft.Debug.TypeCheck.CastReference<AkSoundBankHierarchySound>(dic.Value).PrepareForExtraction(bank);
				}
			}
		}
	};
}
