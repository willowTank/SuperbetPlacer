using MasterDevs.ChromeDevTools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MasterDevs.ChromeDevTools.Protocol.Chrome.Input
{
	/// <summary>
	/// Dispatches a key event to the page.
	/// </summary>
	[Command(ProtocolName.Input.DispatchKeyEvent)]
	[SupportedBy("Chrome")]
	public class DispatchKeyEventCommand: ICommand<DispatchKeyEventCommandResponse>
	{
		/// <summary>
	/// Gets or sets Type of the key event.
		/// </summary>
		public string Type { get; set; }
		/// <summary>
	/// Gets or sets Bit field representing pressed modifier keys. Alt=1, Ctrl=2, Meta/Command=4, Shift=8
	/// (default: 0).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public long? Modifiers { get; set; }
		/// <summary>
	/// Gets or sets Time at which the event occurred.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public double Timestamp { get; set; }
		/// <summary>
	/// Gets or sets Text as generated by processing a virtual key code with a keyboard layout. Not needed for
	/// for `keyUp` and `rawKeyDown` events (default: "")
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Text { get; set; }
		/// <summary>
	/// Gets or sets Text that would have been generated by the keyboard if no modifiers were pressed (except for
	/// shift). Useful for shortcut (accelerator) key handling (default: "").
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string UnmodifiedText { get; set; }
		/// <summary>
	/// Gets or sets Unique key identifier (e.g., 'U+0041') (default: "").
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string KeyIdentifier { get; set; }
		/// <summary>
	/// Gets or sets Unique DOM defined string value for each physical key (e.g., 'KeyA') (default: "").
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Code { get; set; }
		/// <summary>
	/// Gets or sets Unique DOM defined string value describing the meaning of the key in the context of active
	/// modifiers, keyboard layout, etc (e.g., 'AltGr') (default: "").
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Key { get; set; }
		/// <summary>
	/// Gets or sets Windows virtual key code (default: 0).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public long? WindowsVirtualKeyCode { get; set; }
		/// <summary>
	/// Gets or sets Native virtual key code (default: 0).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public long? NativeVirtualKeyCode { get; set; }
		/// <summary>
	/// Gets or sets Whether the event was generated from auto repeat (default: false).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? AutoRepeat { get; set; }
		/// <summary>
	/// Gets or sets Whether the event was generated from the keypad (default: false).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsKeypad { get; set; }
		/// <summary>
	/// Gets or sets Whether the event was a system key event (default: false).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsSystemKey { get; set; }
		/// <summary>
	/// Gets or sets Whether the event was from the left or right side of the keyboard. 1=Left, 2=Right (default:
	/// 0).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public long? Location { get; set; }

		/// <summary>
		/// Editing commands to send with the key event (e.g., 'selectAll') (default: []). These are related to but not equal the command names used in document.execCommand and NSStandardKeyBindingResponding. See https://source.chromium.org/chromium/chromium/src/+/main:third_party/blink/renderer/core/editing/commands/editor_command_names.h for valid command names.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string[] Commands { get; set; }

	}
}