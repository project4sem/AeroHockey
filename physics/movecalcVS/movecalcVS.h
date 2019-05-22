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
	
	struct Coord MOVECALC_API movecalc(double _factor_v, double _factor_w, double _dt, double _r, struct Coord _coord);
	struct Coord MOVECALC_API hit(double k, double m, double mu, double r, struct Coord coord, struct Obj obj);
	struct Coord MOVECALC_API boost(double v_max, struct Coord _coord, struct Obj obj);
}



