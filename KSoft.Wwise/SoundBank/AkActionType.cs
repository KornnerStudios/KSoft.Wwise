﻿
namespace KSoft.Wwise.SoundBank
{
	[System.Reflection.Obfuscation(Exclude=true)]
	public enum AkActionType : uint
	{
		None = 0x0,
		SetState = 0x12020,
		BypassFX_M = 0x70010,
		BypassFX_O = 0x70011,
		ResetBypassFX_M = 0x80010,
		ResetBypassFX_O = 0x80011,
		ResetBypassFX_ALL = 0x80020,
		ResetBypassFX_ALL_O = 0x80021,
		ResetBypassFX_AE = 0x80040,
		ResetBypassFX_AE_O = 0x80041,
		SetSwitch = 0x60001,
		SetRTPC = 0x61001,
		UseState_E = 0x10010,
		UnuseState_E = 0x11010,
		Play = 0x4011,
		PlayAndContinue = 0x5011,
		Stop_E = 0x1010,
		Stop_E_O = 0x1011,
		Stop_ALL = 0x1020,
		Stop_ALL_O = 0x1021,
		Stop_AE = 0x1040,
		Stop_AE_O = 0x1041,
		Pause_E = 0x2010,
		Pause_E_O = 0x2011,
		Pause_ALL = 0x2020,
		Pause_ALL_O = 0x2021,
		Pause_AE = 0x2040,
		Pause_AE_O = 0x2041,
		Resume_E = 0x3010,
		Resume_E_O = 0x3011,
		Resume_ALL = 0x3020,
		Resume_ALL_O = 0x3021,
		Resume_AE = 0x3040,
		Resume_AE_O = 0x3041,
		Break_E = 0x90010,
		Break_E_O = 0x90011,
		Mute_M = 0x6010,
		Mute_O = 0x6011,
		Unmute_M = 0x7010,
		Unmute_O = 0x7011,
		Unmute_ALL = 0x7020,
		Unmute_ALL_O = 0x7021,
		Unmute_AE = 0x7040,
		Unmute_AE_O = 0x7041,
		SetVolume_M = 0xA010,
		SetVolume_O = 0xA011,
		ResetVolume_M = 0xB010,
		ResetVolume_O = 0xB011,
		ResetVolume_ALL = 0xB020,
		ResetVolume_ALL_O = 0xB021,
		ResetVolume_AE = 0xB040,
		ResetVolume_AE_O = 0xB041,
		SetPitch_M = 0x8010,
		SetPitch_O = 0x8011,
		ResetPitch_M = 0x9010,
		ResetPitch_O = 0x9011,
		ResetPitch_ALL = 0x9020,
		ResetPitch_ALL_O = 0x9021,
		ResetPitch_AE = 0x9040,
		ResetPitch_AE_O = 0x9041,
		SetLFE_M = 0xC010,
		SetLFE_O = 0xC011,
		ResetLFE_M = 0xD010,
		ResetLFE_O = 0xD011,
		ResetLFE_ALL = 0xD020,
		ResetLFE_ALL_O = 0xD021,
		ResetLFE_AE = 0xD040,
		ResetLFE_AE_O = 0xD041,
		SetLPF_M = 0xE010,
		SetLPF_O = 0xE011,
		ResetLPF_M = 0xF010,
		ResetLPF_O = 0xF011,
		ResetLPF_ALL = 0xF020,
		ResetLPF_ALL_O = 0xF021,
		ResetLPF_AE = 0xF040,
		ResetLPF_AE_O = 0xF041,
		StopEvent = 0x20081,
		PauseEvent = 0x30081,
		ResumeEvent = 0x40081,
		Duck = 0x50100,
		Trigger = 0xA0000,
		Trigger_O = 0xA0001,
	};
}