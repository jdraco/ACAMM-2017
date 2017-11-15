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

	public enum CPre{
		SG,
		MY,
		PH,
		TH,
		VN,
		BN,
		CM,
		ID,
		LS,
		MYR,
		AARM
	}
	public static CP cp = CP.SG;
	public static CPre cp2 = CPre.SG;

	public enum SS{
		ACAMM,
		ASMAM,
		ACAMMSP
	}
	public static SS ss = SS.ACAMM;
	public static authentication_Manager authManager;
}
