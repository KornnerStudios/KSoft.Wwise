﻿using System;
using System.Collections.Generic;
using Contract = System.Diagnostics.Contracts.Contract;

namespace KSoft.Wwise.SoundBank
{
	using HircTypeStreamer8 = IO.EnumBinaryStreamer<HircType, byte>;
	using HircTypeStreamer32 = IO.EnumBinaryStreamer<HircType, uint>;

	partial class AkSoundBankObjectBase
	{
		static readonly Values.GroupTagData32 kHierarchySignature =
					new Values.GroupTagData32("HIRC", "audiokinetic_hierarchy"); // BankHierarchyChunkID

		static AkSoundBankObjectBase NewHIRC(uint generatorVersion)
		{
			return new AkSoundBankHierarchy();
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
				uint sdk_ver = (s.Owner as AkSoundBank).SdkVersion;

				s.Stream(ref Type, AkVersion.HircTypeIs8bit(sdk_ver)
					? HircTypeStreamer8.Instance
					: HircTypeStreamer32.Instance);
				s.Stream(ref SectionSize);
			}
			#endregion
		};

		Dictionary<HircType, Dictionary<uint, AkSoundBankHierarchyObjectBase>> mObjects = 
			new Dictionary<HircType, Dictionary<uint, AkSoundBankHierarchyObjectBase>>();
		Dictionary<uint, AkSoundBankHierarchyObjectBase> mIdToObject = 
			new Dictionary<uint, AkSoundBankHierarchyObjectBase>();

		public void CopyObjectsTo(FilePackage.AkFilePackageExtractor extractor)
		{
			foreach (var kv in mObjects)
			{
				var type = kv.Key;

				if (type == HircType.Attenuation)
					continue;

				Dictionary<uint, AkSoundBankHierarchyObjectBase> dic;
				if (!extractor.mObjects.TryGetValue(type, out dic))
					extractor.mObjects.Add(type, dic = new Dictionary<uint, AkSoundBankHierarchyObjectBase>());

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
			Dictionary<uint, AkSoundBankHierarchyObjectBase> dic;
			if (!mObjects.TryGetValue(type, out dic))
				mObjects.Add(type, dic = new Dictionary<uint, AkSoundBankHierarchyObjectBase>());

			dic.Add(obj.ID, obj);
			mIdToObject.Add(obj.ID, obj);
		}

		#region IEndianStreamSerializable Members
		void SerializeItem(IO.EndianStream s, AKBKSubHircSection section)
		{
			Contract.Assert(s.IsReading);

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
			var bank = s.Owner as AkSoundBank;

			using (s.EnterVirtualBufferWithBookmark(header.ChunkSize))
			{
				for (int x = 0, num_hirc_items = s.Reader.ReadInt32(); x < num_hirc_items; x++)
				{
					var section = new AKBKSubHircSection(); section.Serialize(s);

					SerializeItem(s, section);
				}
			}
		}
		public override void Serialize(IO.EndianStream s, AkSubchunkHeader header)
		{
			if (s.IsReading)
				FromStream(s, header);
		}
		#endregion

		internal void PrepareForExtraction(AkSoundBank bank)
		{
			foreach (var kv in mObjects)
			{
				if (kv.Key != HircType.Sound)
					continue;

				foreach(var dic in kv.Value)
					((AkSoundBankHierarchySound)dic.Value).PrepareForExtraction(bank);
			}
		}
	};
}