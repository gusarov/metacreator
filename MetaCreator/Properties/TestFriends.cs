using System.Runtime.CompilerServices;

#if DEBUG
[assembly: InternalsVisibleTo("MetaCreator UnitTest")]
#else
[assembly: InternalsVisibleTo(
	"MetaCreator UnitTest, PublicKey="+
	"002400000480000094000000060200" +
	"000024000052534131000400000100" +
	"010053949b8cac7eee61b64d611ad7" +
	"7a04ad367f22e83dddadfe86ccaa78" +
	"63e70dfcb7e6c26163dbaf76636491" +
	"838d792e88a80fe6d38e09faacc79f" +
	"a3adac71f31dd496bf11ab5b28ef73" +
	"ad3df52382048ea2ab21f5af2e1477" +
	"a094201aca4fb5cc26e0f49f4abc38" +
	"2e2772530f9620ad957a4c7eb61793" +
	"2ed2ad918405febd1fb7")]
#endif