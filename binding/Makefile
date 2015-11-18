BTOUCH=/Developer/MonoTouch/usr/bin/btouch
BTOUCHN=/Developer/MonoTouch/usr/bin/btouch-native
OUT_DIR=../../Build

all: ZBar.dll

libzbar.a:
	@echo "You'll need to obtain a copy of libzbar.a"
	@echo "from http://zbar.sourceforge.net/iphone/"
	@echo
	@open http://zbar.sourceforge.net/iphone/
	exit 1

ZBar.dll: Makefile libzbar.a
	@echo "TODO"
	exit 1
	$(BTOUCH) ApiDefinition.cs AssemblyInfo.cs


unified: libzbar.a
	$(BTOUCHN) --new-style ApiDefinition.cs StructsAndEnums.cs libzbar.linkwith.cs --out:$(OUT_DIR)/ZBar.dll --outdir=$(OUT_DIR) \
	-x=ZBarImageScanner.cs -x=ZBarSDK.cs -x=ZBarSymbol.cs -x=ZBarSymbolSet.cs -link-with=libzbar.a,libzbar.a

clean:
	-rm -f *.a *.dll
