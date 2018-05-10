// --------------------------------------------------------------------------------------------
// Version: MPL 1.1/GPL 2.0/LGPL 2.1
// 
// The contents of this file are subject to the Mozilla Public License Version
// 1.1 (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
// http://www.mozilla.org/MPL/
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
// for the specific language governing rights and limitations under the
// License.
// 
// <remarks>
// Generated by IDLImporter from file nsIRemoteService.idl
// 
// You should use these interfaces when you access the COM objects defined in the mentioned
// IDL/IDH file.
// </remarks>
// --------------------------------------------------------------------------------------------
namespace Gecko
{
	using System;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;
	using System.Runtime.CompilerServices;
	
	
	/// <summary>
    /// Start and stop the remote service (xremote/phremote), and register
    /// windows with the service for backwards compatibility with old xremote
    /// clients.
    ///
    /// @status FLUID This interface is not frozen and is not intended for embedders
    /// who want a frozen API. If you are an embedder and need this
    /// functionality, contact Benjamin Smedberg about the possibility
    /// of freezing the functionality you need.
    /// </summary>
	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("a2240f6a-f1e4-4548-9e1a-6f3bc9b2426c")]
	public interface nsIRemoteService
	{
		
		/// <summary>
        /// Start the remote service. This should not be done until app startup
        /// appears to have been successful.
        ///
        /// @param appName     (Required) Sets a window property identifying the
        /// application.
        /// @param profileName (May be null) Sets a window property identifying the
        /// profile name.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Startup([MarshalAs(UnmanagedType.LPStr)] string appName, [MarshalAs(UnmanagedType.LPStr)] string profileName);
		
		/// <summary>
        /// Register a XUL window with the xremote service. The window will be
        /// configured to accept incoming remote requests. If this method is called
        /// before startup(), the registration will happen once startup() is called.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void RegisterWindow([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow);
		
		/// <summary>
        /// Stop the remote service from accepting additional requests.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Shutdown();
	}
}
