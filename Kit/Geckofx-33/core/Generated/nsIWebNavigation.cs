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
// Generated by IDLImporter from file nsIWebNavigation.idl
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
    /// The nsIWebNavigation interface defines an interface for navigating the web.
    /// It provides methods and attributes to direct an object to navigate to a new
    /// location, stop or restart an in process load, or determine where the object
    /// has previously gone.
    /// </summary>
	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("b7568a50-4c50-442c-a6be-3a340a48d89a")]
	public interface nsIWebNavigation
	{
		
		/// <summary>
        /// Indicates if the object can go back.  If true this indicates that
        /// there is back session history available for navigation.
        /// </summary>
		[return: MarshalAs(UnmanagedType.U1)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		bool GetCanGoBackAttribute();
		
		/// <summary>
        /// Indicates if the object can go forward.  If true this indicates that
        /// there is forward session history available for navigation
        /// </summary>
		[return: MarshalAs(UnmanagedType.U1)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		bool GetCanGoForwardAttribute();
		
		/// <summary>
        /// Tells the object to navigate to the previous session history item.  When a
        /// page is loaded from session history, all content is loaded from the cache
        /// (if available) and page state (such as form values and scroll position) is
        /// restored.
        ///
        /// @throw NS_ERROR_UNEXPECTED
        /// Indicates that the call was unexpected at this time, which implies
        /// that canGoBack is false.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void GoBack();
		
		/// <summary>
        /// Tells the object to navigate to the next session history item.  When a
        /// page is loaded from session history, all content is loaded from the cache
        /// (if available) and page state (such as form values and scroll position) is
        /// restored.
        ///
        /// @throw NS_ERROR_UNEXPECTED
        /// Indicates that the call was unexpected at this time, which implies
        /// that canGoForward is false.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void GoForward();
		
		/// <summary>
        /// Tells the object to navigate to the session history item at a given index.
        ///
        /// @throw NS_ERROR_UNEXPECTED
        /// Indicates that the call was unexpected at this time, which implies
        /// that session history entry at the given index does not exist.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void GotoIndex(int index);
		
		/// <summary>
        /// Loads a given URI.  This will give priority to loading the requested URI
        /// in the object implementing	this interface.  If it can't be loaded here
        /// however, the URI dispatcher will go through its normal process of content
        /// loading.
        ///
        /// @param aURI
        /// The URI string to load.  For HTTP and FTP URLs and possibly others,
        /// characters above U+007F will be converted to UTF-8 and then URL-
        /// escaped per the rules of RFC 2396.
        /// @param aLoadFlags
        /// Flags modifying load behaviour.  This parameter is a bitwise
        /// combination of the load flags defined above.  (Undefined bits are
        /// reserved for future use.)  Generally you will pass LOAD_FLAGS_NONE
        /// for this parameter.
        /// @param aReferrer
        /// The referring URI.  If this argument is null, then the referring
        /// URI will be inferred internally.
        /// @param aPostData
        /// If the URI corresponds to a HTTP request, then this stream is
        /// appended directly to the HTTP request headers.  It may be prefixed
        /// with additional HTTP headers.  This stream must contain a "\r\n"
        /// sequence separating any HTTP headers from the HTTP request body.
        /// This parameter is optional and may be null.
        /// @param aHeaders
        /// If the URI corresponds to a HTTP request, then any HTTP headers
        /// contained in this stream are set on the HTTP request.  The HTTP
        /// header stream is formatted as:
        /// ( HEADER "\r\n" )*
        /// This parameter is optional and may be null.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void LoadURI([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Gecko.CustomMarshalers.WStringMarshaler")] string aURI, uint aLoadFlags, [MarshalAs(UnmanagedType.Interface)] nsIURI aReferrer, [MarshalAs(UnmanagedType.Interface)] nsIInputStream aPostData, [MarshalAs(UnmanagedType.Interface)] nsIInputStream aHeaders);
		
		/// <summary>
        /// Loads a given URI.  This will give priority to loading the requested URI
        /// in the object implementing this interface.  If it can't be loaded here
        /// however, the URI dispatcher will go through its normal process of content
        /// loading.
        /// Behaves like loadURI, except an additional parameter is provided to supply
        /// a base URI to be used in specific situations where one cannot be inferred
        /// by other means, for example when this is called to view selection source.
        /// Outside of these situations, the behaviour of this function is no
        /// different to loadURI.
        ///
        /// @param aURI
        /// The URI string to load.  For HTTP and FTP URLs and possibly others,
        /// characters above U+007F will be converted to UTF-8 and then URL-
        /// escaped per the rules of RFC 2396.
        /// @param aLoadFlags
        /// Flags modifying load behaviour.  This parameter is a bitwise
        /// combination of the load flags defined above.  (Undefined bits are
        /// reserved for future use.)  Generally you will pass LOAD_FLAGS_NONE
        /// for this parameter.
        /// @param aReferrer
        /// The referring URI.  If this argument is null, then the referring
        /// URI will be inferred internally.
        /// @param aPostData
        /// If the URI corresponds to a HTTP request, then this stream is
        /// appended directly to the HTTP request headers.  It may be prefixed
        /// with additional HTTP headers.  This stream must contain a "\r\n"
        /// sequence separating any HTTP headers from the HTTP request body.
        /// This parameter is optional and may be null.
        /// @param aHeaders
        /// If the URI corresponds to a HTTP request, then any HTTP headers
        /// contained in this stream are set on the HTTP request.  The HTTP
        /// header stream is formatted as:
        /// ( HEADER "\r\n" )*
        /// This parameter is optional and may be null.
        /// @param aBaseURI
        /// Set to indicate a base URI to be associated with the load. Note
        /// that at present this argument is only used with view-source aURIs
        /// and cannot be used to resolve aURI.
        /// This parameter is optional and may be null.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void LoadURIWithBase([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Gecko.CustomMarshalers.WStringMarshaler")] string aURI, uint aLoadFlags, [MarshalAs(UnmanagedType.Interface)] nsIURI aReferrer, [MarshalAs(UnmanagedType.Interface)] nsIInputStream aPostData, [MarshalAs(UnmanagedType.Interface)] nsIInputStream aHeaders, [MarshalAs(UnmanagedType.Interface)] nsIURI aBaseURI);
		
		/// <summary>
        /// Tells the Object to reload the current page.  There may be cases where the
        /// user will be asked to confirm the reload (for example, when it is
        /// determined that the request is non-idempotent).
        ///
        /// @param aReloadFlags
        /// Flags modifying load behaviour.  This parameter is a bitwise
        /// combination of the Load Flags defined above.  (Undefined bits are
        /// reserved for future use.)  Generally you will pass LOAD_FLAGS_NONE
        /// for this parameter.
        ///
        /// @throw NS_BINDING_ABORTED
        /// Indicating that the user canceled the reload.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Reload(uint aReloadFlags);
		
		/// <summary>
        /// Stops a load of a URI.
        ///
        /// @param aStopFlags
        /// This parameter is one of the stop flags defined above.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void Stop(uint aStopFlags);
		
		/// <summary>
        /// Retrieves the current DOM document for the frame, or lazily creates a
        /// blank document if there is none.  This attribute never returns null except
        /// for unexpected error situations.
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsIDOMDocument GetDocumentAttribute();
		
		/// <summary>
        /// The currently loaded URI or null.
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsIURI GetCurrentURIAttribute();
		
		/// <summary>
        /// The referring URI for the currently loaded URI or null.
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsIURI GetReferringURIAttribute();
		
		/// <summary>
        /// The session history object used by this web navigation instance.
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsISHistory GetSessionHistoryAttribute();
		
		/// <summary>
        /// The session history object used by this web navigation instance.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void SetSessionHistoryAttribute([MarshalAs(UnmanagedType.Interface)] nsISHistory aSessionHistory);
	}
	
	/// <summary>nsIWebNavigationConsts </summary>
	public class nsIWebNavigationConsts
	{
		
		// <summary>
        // This flags defines the range of bits that may be specified.  Flags
        // outside this range may be used, but may not be passed to Reload().
        // </summary>
		public const ulong LOAD_FLAGS_MASK = 0xffff;
		
		// <summary>
        // This is the default value for the load flags parameter.
        // </summary>
		public const ulong LOAD_FLAGS_NONE = 0x0000;
		
		// <summary>
        // This flag specifies that the load should have the semantics of an HTML
        // Meta-refresh tag (i.e., that the cache should be bypassed).  This flag
        // is only applicable to loadURI.
        // XXX the meaning of this flag is poorly defined.
        // XXX no one uses this, so we should probably deprecate and remove it.
        // </summary>
		public const ulong LOAD_FLAGS_IS_REFRESH = 0x0010;
		
		// <summary>
        // This flag specifies that the load should have the semantics of a link
        // click.  This flag is only applicable to loadURI.
        // XXX the meaning of this flag is poorly defined.
        // </summary>
		public const ulong LOAD_FLAGS_IS_LINK = 0x0020;
		
		// <summary>
        // This flag specifies that history should not be updated.  This flag is only
        // applicable to loadURI.
        // </summary>
		public const ulong LOAD_FLAGS_BYPASS_HISTORY = 0x0040;
		
		// <summary>
        // This flag specifies that any existing history entry should be replaced.
        // This flag is only applicable to loadURI.
        // </summary>
		public const ulong LOAD_FLAGS_REPLACE_HISTORY = 0x0080;
		
		// <summary>
        // This flag specifies that the local web cache should be bypassed, but an
        // intermediate proxy cache could still be used to satisfy the load.
        // </summary>
		public const ulong LOAD_FLAGS_BYPASS_CACHE = 0x0100;
		
		// <summary>
        // This flag specifies that any intermediate proxy caches should be bypassed
        // (i.e., that the content should be loaded from the origin server).
        // </summary>
		public const ulong LOAD_FLAGS_BYPASS_PROXY = 0x0200;
		
		// <summary>
        // This flag specifies that a reload was triggered as a result of detecting
        // an incorrect character encoding while parsing a previously loaded
        // document.
        // </summary>
		public const ulong LOAD_FLAGS_CHARSET_CHANGE = 0x0400;
		
		// <summary>
        // If this flag is set, Stop() will be called before the load starts
        // and will stop both content and network activity (the default is to
        // only stop network activity).  Effectively, this passes the
        // STOP_CONTENT flag to Stop(), in addition to the STOP_NETWORK flag.
        // </summary>
		public const ulong LOAD_FLAGS_STOP_CONTENT = 0x0800;
		
		// <summary>
        // A hint this load was prompted by an external program: take care!
        // </summary>
		public const ulong LOAD_FLAGS_FROM_EXTERNAL = 0x1000;
		
		// <summary>
        //This flag is set when a user explicitly disables the Mixed Content
        //    Blocker, and allows Mixed Content to load on an https page.
        // </summary>
		public const ulong LOAD_FLAGS_ALLOW_MIXED_CONTENT = 0x2000;
		
		// <summary>
        // This flag specifies that this is the first load in this object.
        // Set with care, since setting incorrectly can cause us to assume that
        // nothing was actually loaded in this object if the load ends up being
        // handled by an external application.  This flag must not be passed to
        // Reload.
        // </summary>
		public const ulong LOAD_FLAGS_FIRST_LOAD = 0x4000;
		
		// <summary>
        // This flag specifies that the load should not be subject to popup
        // blocking checks.  This flag must not be passed to Reload.
        // </summary>
		public const ulong LOAD_FLAGS_ALLOW_POPUPS = 0x8000;
		
		// <summary>
        // This flag specifies that the URI classifier should not be checked for
        // this load.  This flag must not be passed to Reload.
        // </summary>
		public const ulong LOAD_FLAGS_BYPASS_CLASSIFIER = 0x10000;
		
		// <summary>
        // Force relevant cookies to be sent with this load even if normally they
        // wouldn't be.
        // </summary>
		public const ulong LOAD_FLAGS_FORCE_ALLOW_COOKIES = 0x20000;
		
		// <summary>
        // Prevent the owner principal from being inherited for this load.
        // </summary>
		public const ulong LOAD_FLAGS_DISALLOW_INHERIT_OWNER = 0x40000;
		
		// <summary>
        // This flag specifies that the URI may be submitted to a third-party
        // server for correction. This should only be applied to non-sensitive
        // URIs entered by users.  This flag must not be passed to Reload.
        // </summary>
		public const ulong LOAD_FLAGS_ALLOW_THIRD_PARTY_FIXUP = 0x100000;
		
		// <summary>
        // This flag specifies that common scheme typos should be corrected.
        // </summary>
		public const ulong LOAD_FLAGS_FIXUP_SCHEME_TYPOS = 0x200000;
		
		// <summary>
        // This flag specifies that all network activity should be stopped.  This
        // includes both active network loads and pending META-refreshes.
        // </summary>
		public const ulong STOP_NETWORK = 0x01;
		
		// <summary>
        // This flag specifies that all content activity should be stopped.  This
        // includes animated images, plugins and pending Javascript timeouts.
        // </summary>
		public const ulong STOP_CONTENT = 0x02;
		
		// <summary>
        // This flag specifies that all activity should be stopped.
        // </summary>
		public const ulong STOP_ALL = 0x03;
	}
}
