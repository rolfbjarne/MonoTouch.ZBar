using System;
using System.Collections;
using System.Drawing;

using CoreGraphics;
using ObjCRuntime;
using Foundation;
using UIKit;

namespace ZBar
{
	/******************************************************************************
	 	ZBar Static Methods and Constants
	*/
	 
	[Static]
	interface ZBarSDK
	{
		// extern NSString* const ZBarReaderControllerResults;
		[Field ("ZBarReaderControllerResults", "__Internal")]
		NSString BarcodeResultsKey { get; }
		
		/** retrieve runtime library version information.
		 * @param major set to the running major version (unless NULL)
		 * @param minor set to the running minor version (unless NULL)
		 * @returns 0
		 */
		// extern int zbar_version(unsigned *major, unsigned *minor);
		// NOT SURE HOW TO DO THIS HERE - I have manually linked this
		// within the addition static class 'ZBarSDK'.
	}
	
	/*******************************************************************************
		ZBarSymbol
		
		MonoTouch wrapper for Obj-C wrapper for ZBar result types
	*/
	
	// @interface ZBarSymbol : NSObject
	[BaseType (typeof (NSObject))]
	interface ZBarSymbol 
	{
		// @property (readonly, nonatomic) zbar_symbol_type_t type;
		[Export ("type")]
		ZBarSymbolType SymbolType { get; }
		
		// @property (readonly, nonatomic) NSString *typeName;
		[Export ("typeName")]
		string BarcodeTypeName { get; }
		
		// @property (readonly, nonatomic) NSUInteger configMask;
		[Export ("configMask")]
		ZBarConfig Configuration { get; }
		
		// @property (readonly, nonatomic) NSUInteger modifierMask;
		[Export ("modifierMask")]
		ZBarModifier Modifiers { get; }
		
		// @property (readonly, nonatomic) NSString *data;
		[Export ("data")]
		string Data { get; }
		
		// @property (readonly, nonatomic) int quality;
		[Export ("quality")]
		int Quality { get; }
		
		// @property (readonly, nonatomic) int count;
		[Export ("count")]
		int Count { get; }
		
		// @property (readonly, nonatomic) zbar_orientation_t orientation;
		// [Export ("orientation")]
		// ZBarOrientation Orientation { get; }
		// TODO: See note in enum definition for ZBarOrientation2.
		
		// @property (readonly, nonatomic) ZBarSymbolSet *components;
		// [Export ("components")]
		// TODO - See comment below on ZBarSymbolSet
		// ZBarSymbolSet Components { get; }
		
		// @property (readonly, nonatomic) const zbar_symbol_t *zbarSymbol;
		// [Export ("zbarSymbol")]
		// ? Symbol { get; }
		
		// @property (readonly, nonatomic) CGRect bounds;
		[Export ("bounds")]
		RectangleF Bounds { get; }

		// + (NSString*) nameForType: (zbar_symbol_type_t) type;
	}
	
	/*******************************************************************************
		ZBarSymbolSet
		
		MonoTouch wrapper for Obj-C wrapper for ZBar result types
	*/
	
	// @interface ZBarSymbolSet : NSObject <NSFastEnumeration>
	[BaseType (typeof(NSObject))]
	interface ZBarSymbolSet
	{
		// @property (readonly, nonatomic) int count;
		[Export("count")]
		int Count { get; }
		
		// @property (readonly, nonatomic) const zbar_symbol_set_t *zbarSymbolSet;
		[Export("zbarSymbolSet")]
		IntPtr InnerNativeSymbolSetHandle{ get; }
		
		// @property (nonatomic) BOOL filterSymbols;
		[Export("filterSymbols")]
		bool FilterEnabled { get; set; }
	}

	/*******************************************************************************
		ZBarReaderDelegate

		The delegate receives messages about the results of a scan.
	*/
	
	// @protocol ZBarReaderDelegate <UIImagePickerControllerDelegate>
	[BaseType (typeof (UIImagePickerControllerDelegate))]
	[Model]
	[Protocol]
	interface ZBarReaderDelegate 
	{
		// called when no barcode is found in an image selected by the user.
		// if retry is NO, the delegate *must* dismiss the controller
		// - (void) readerControllerDidFailToRead: (ZBarReaderController*) reader withRetry: (BOOL) retry;	
		[Export ("readerControllerDidFailToRead:withRetry:")]
		[EventArgs ("ReadBarcodeOperation")] // Produces 'ReadBarcodeOperationEventArgs'
		[EventName ("BarcodeReadFailed")] 
		void BarcodeNotFound (ZBarReaderViewController reader, bool willRetry);
	}
	
	/*******************************************************************************
		ZBarReaderViewController

		This ViewController subclass runs the ZBar scanner, detects barcodes, and
		notifies its delegate of what it found.
		
		This is the drop in video scanning replacement for ZBarReaderController.
		This is a thin controller around a ZBarReaderView that adds the UI controls and 
		select functionality offered by ZBarReaderController. Automatically falls back to a 
		ZBarReaderController if video APIs are unavailable (eg for OS < 4.0)
		
	*/
	
	// @interface ZBarReaderViewController : UIViewController
	[BaseType (typeof (UIViewController), 
	          Delegates=new string [] { "WeakReaderDelegate" }, 
              Events=new Type [] {typeof(ZBarReaderDelegate)})]
	interface ZBarReaderViewController
	{
	
		// access to configure image scanner
		// @property (nonatomic, readonly) ZBarImageScanner *scanner;
		[Export ("scanner")]
		ZBarImageScanner Scanner { get; }
		
		// barcode result recipient
		// @property (nonatomic, assign) id <ZBarReaderDelegate> readerDelegate;
		[Export ("readerDelegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakReaderDelegate { get; set; }
		
		[Wrap ("WeakReaderDelegate")][NullAllowed]
		ZBarReaderDelegate ReaderDelegate { get; set; }
	
		// whether to use alternate control set
		// @property (nonatomic) BOOL showsZBarControls;
		[Export ("showsZBarControls")]
		bool ShowsZBarControls { get; set; }
		
		// whether to show the green tracking box.  note that, even when
		// enabled, the box will only be visible when scanning EAN and I2/5.
		// @property (nonatomic) BOOL tracksSymbols;
		[Export ("tracksSymbols")]
		bool ShowBarcodeTracker { get; set; }
		
		// interface orientation support.  bit-mask of accepted orientations.
		// see eg ZBarOrientationMask() and ZBarOrientationMaskAll
		// @property (nonatomic) NSUInteger supportedOrientationsMask;
		[Export ("supportedOrientationsMask")]
		ZBarOrientation SupportedOrientations { get; set; }
		
		// crop images for scanning.  the image will be cropped to this
		// rectangle before scanning.  the rectangle is normalized to the
		// image size and aspect ratio; useful values will place the rectangle
		// between 0 and 1 on each axis, where the x-axis corresponds to the
		// image major axis.  defaults to the full image (0, 0, 1, 1).
		// @property (nonatomic) CGRect scanCrop;
		[Export ("scanCrop")]
		RectangleF ScanBounds { get; set; }
		
		// provide a custom overlay.  note that this can be used with
		// showsZBarControls enabled (but not if you want backward compatibility)
		// @property (nonatomic, retain) UIView *cameraOverlayView;
		[Export ("cameraOverlayView", ArgumentSemantic.Retain)]
		UIView Overlay { get; set; }
		
		// transform applied to the preview image.
		// @property (nonatomic) CGAffineTransform cameraViewTransform;
		[Export ("cameraViewTransform")]
		CGAffineTransform CameraViewTransform { get; set; }
		
		// display the built-in help browser.  the argument will be passed to
		// the onZBarHelp() javascript function.
		// - (void) showHelpWithReason: (NSString*) reason;
		[Export ("showHelpWithReason:")]
		void ShowHelp (string reason);
		
		// capture the next frame and send it over the usual delegate path.
		// - (void) takePicture;
		[Export ("takePicture")]
		void TakePicture ();
		
		// TODO IF NEEDED
		// these attempt to emulate UIImagePickerController
		// + (BOOL) isCameraDeviceAvailable: (UIImagePickerControllerCameraDevice) cameraDevice;
		// + (BOOL) isFlashAvailableForCameraDevice: (UIImagePickerControllerCameraDevice) cameraDevice;
		// + (NSArray*) availableCaptureModesForCameraDevice: (UIImagePickerControllerCameraDevice) cameraDevice;
		// @property(nonatomic) UIImagePickerControllerCameraDevice cameraDevice;
		// @property(nonatomic) UIImagePickerControllerCameraFlashMode cameraFlashMode;
		// @property(nonatomic) UIImagePickerControllerCameraCaptureMode cameraCaptureMode;
		// @property(nonatomic) UIImagePickerControllerQualityType videoQuality;
		
		// direct access to the ZBarReaderView
		// @property (nonatomic, readonly) ZBarReaderView *readerView;
	}
	
	/*******************************************************************************
		ZBarReaderController

		This ViewController subclass runs the ZBar scanner, detects barcodes, and
		notifies its delegate of what it found.
	*/
	// Not sure how to bind this as it implements 2 protocols that are from the MonoTouch BCL that
	// are not declared as interfaces. Probably have to repeat interfaces in my binding definitions
	// but this is not needed anyway so ignoring for now.
	/*
	[BaseType (typeof (UIImagePickerController))]
	interface ZBarReaderController : UINavigationControllerDelegate, UIImagePickerControllerDelegate
	{
		
		// OLD RedLaser example below 
		[Export ("pauseScanning")]
		void PauseScanning ();
		
		[Export ("resumeScanning")]
		void ResumeScanning ();
		
		[Export ("clearResultsSet")]
		void ClearResultsSet ();
		
		[Export ("doneScanning")]
		void DoneScanning ();
		
		[Export ("returnBarcode:withInfo:")]
		void ReturnBarcode (string ean, NSDictionary info);
		
		[Export ("hasFlash")]
		bool FlashEnabled { get; [Bind ("turnFlash:")] set; }
		
		[Export ("overlay", ArgumentSemantic.Retain)]
		CameraOverlayViewController Overlay { get; set; }
		
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Wrap ("WeakDelegate")]
		BarcodePickerControllerDelegate Delegate { get; set; }
		
		[Export ("scanUPCE", ArgumentSemantic.Assign)]
		bool ScanUPCE { get; set; }
		
		[Export ("scanEAN8", ArgumentSemantic.Assign)]
		bool ScanEAN8 { get; set; }
		
		[Export ("scanEAN13", ArgumentSemantic.Assign)]
		bool ScanEAN13 { get; set; }
		
		[Export ("scanSTICKY", ArgumentSemantic.Assign)]
		bool ScanSTICKY { get; set; }
		
		[Export ("scanQRCODE", ArgumentSemantic.Assign)]
		bool ScanQRCODE { get; set; }
		
		[Export ("scanCODE128", ArgumentSemantic.Assign)]
		bool ScanCODE128 { get; set; }
		
		[Export ("scanCODE39", ArgumentSemantic.Assign)]
		bool ScanCODE39 { get; set; }
		
		[Export ("scanDATAMATRIX", ArgumentSemantic.Assign)]
		bool ScanDATAMATRIX { get; set; }
		
		[Export ("scanITF", ArgumentSemantic.Assign)]
		bool ScanITF { get; set; }
		
		[Export ("scanEAN5", ArgumentSemantic.Assign)]
		bool ScanEAN5 { get; set; }
		
		[Export ("scanEAN2", ArgumentSemantic.Assign)]
		bool ScanEAN2 { get; set; }
		
		[Export ("activeRegion", ArgumentSemantic.Assign)]
		RectangleF ActiveRegion { get; set; }
		
		[Export ("orientation", ArgumentSemantic.Assign)]
		UIImageOrientation Orientation { get; set; }
		
		[Export ("torchState", ArgumentSemantic.Assign)]
		bool TorchState { get; set; }
		
		[Export ("isFocusing", ArgumentSemantic.Assign)]
		bool IsFocusing { get; }
		
		[Export ("useFrontCamera", ArgumentSemantic.Assign)]
		bool UseFrontCamera { get; set; }
	}
	*/
	
	
	/*******************************************************************************
		ZBarSymbolSet
		
		MonoTouch wrapper for Obj-C wrapper for ZBar image scanner
	*/
	
	//	@interface ZBarImageScanner : NSObject
	[BaseType (typeof(NSObject))]
	interface ZBarImageScanner
	{
		
		//	@property (nonatomic) BOOL enableCache;
		[Export ("enableCache")]
		bool CacheEnabled { get; set; }
		
		//	@property (readonly, nonatomic) ZBarSymbolSet *results;
		[Export ("results")]
		ZBarSymbolSet Results { get; }
		
		//	// decoder configuration
		//	- (void) parseConfig: (NSString*) configStr;
		[Export ("parseConfig:")]
		void ParseConfig (string configuration);
		
		
		//	- (void) setSymbology: (zbar_symbol_type_t) symbology
		//	               config: (zbar_config_t) config
		//	                   to: (int) value;
		[Export ("setSymbology:config:to:")]
		void SetSymbolOption (ZBarSymbolType symbolType, ZBarConfig option, int value);
		
		//	
		//	// image scanning interface
		//	- (NSInteger) scanImage: (ZBarImage*) image;
		
	}
}
