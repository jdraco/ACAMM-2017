using UnityEngine;
//init global values here
public static class GlobalValues {
	public enum CP{
		SG,
		TH,
		VN,
		BN,
		CM,
		ID,
		LS,
		MY,
		MYR,
		PH
	}
	public static CP cp = CP.SG;

	public enum SS{
		ACAMM,
		ASMAM,
		ACAMMSP
	}
	public static SS ss = SS.ACAMM;
}
