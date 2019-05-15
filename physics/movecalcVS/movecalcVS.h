#pragma once

#ifdef MOVECALC_EXPORTS
#define MOVECALC_API __declspec(dllexport)
#else
#define MOVECALC_API __declspec(dllexport)
#endif


extern "C"
{
	struct Coord
	{
		double x, y;
		double vx, vy, w;
	};

	struct Ret_factors
	{
		double cx0, cx1, cx2;
		double cy0, cy1, cy2;
	};

	struct Ret_all
	{
		struct Coord coord;
		struct Ret_factors factors;
	};
	
	int MOVECALC_API Her(int a);
	struct Ret_all MOVECALC_API movecalc(double _factor_v, double _factor_w, double _dt, double _r, struct Coord _coord);
}



