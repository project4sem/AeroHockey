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

	struct Obj
	{
		double vx, vy;
		double phi;
	};
	
	int MOVECALC_API Her(int a);
	struct Coord MOVECALC_API movecalc(double _factor_v, double _factor_w, double _dt, double _r, struct Coord _coord);
	struct Coord MOVECALC_API hit(double _k, double _m, double _mu, double _r, struct Coord _coord, struct Obj obj);
}



