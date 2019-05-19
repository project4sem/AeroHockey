#include "windows.h"
#include "stdio.h"
#include "stdlib.h"
#include "math.h"
#include <gsl/gsl_integration.h>
#include "sys/types.h"
#include "movecalcVS.h"

#define _USE_MATH_DEFINES


extern "C"
{

	// hit
	struct Coord current;
	struct Coord ret_hit;
	struct Obj obj;

	double k, m, mu , r_hit;
	double sgn_before, sgn_after;

	struct Coord coord_trans(double phi , double vx_e , double vy_e , struct Coord _coord)
	{
		struct Coord _ret = _coord;
		_ret.vx =  (_coord.vx - vx_e) * cos(phi) + (_coord.vy - vy_e) * sin(phi);
		_ret.vy = -(_coord.vx - vx_e) * sin(phi) + (_coord.vy - vy_e) * cos(phi);
		return _ret;
	}

	struct Coord coord_trans_back(double phi, double vx_e, double vy_e, struct Coord _coord)
	{
		struct Coord _ret = _coord;
		_ret.vx = _coord.vx * cos(phi) + _coord.vy * sin(phi) - vx_e;
		_ret.vy = -_coord.vx * sin(phi) + _coord.vy * cos(phi) - vy_e;
		return _ret;
	}

	double hit_vx(double S)
	{
		double vx;
		if (sgn_before <= 0)
			vx = current.vx + mu * S / m;
		else
			vx = current.vx - mu * S / m;

		return vx;
	}

	double hit_vy()
	{
		return - current.vy * k;
	}

	double hit_w(double S)
	{
		double w;
		if (sgn_before <= 0)
			w = current.w - mu * S / m / r_hit;
		else
			w = current.w + mu * S / m / r_hit;
		
		return w;
	}

	struct Coord MOVECALC_API hit(double _k, double _m, double _mu , double _r, struct Coord _coord, struct Obj obj)
	{
		k = _k;
		m = _m;
		mu = _mu;
		r_hit = _r;

		obj.phi = M_PI / 2 + obj.phi / 180 * M_PI;
		obj.vx = _coord.vx + obj.vx;
		obj.vy = _coord.vy + obj.vy;

		current = coord_trans(obj.phi , obj.vx , obj.vy , _coord);
		
		if (current.vy >= 0)
		{
			obj.phi = obj.phi - M_PI;
			current = coord_trans( obj.phi, obj.vx, obj.vy, _coord);
		}
			

		printf("vx = %lf   vy = %lf\n", current.vx, current.vy);

		double S = m * (k + 1) * fabs(current.vy);

		sgn_before = current.vx - current.w * r_hit;

		ret_hit.x = current.x;
		ret_hit.y = current.y;

		ret_hit.vx = hit_vx(S);
		ret_hit.vy = hit_vy();
		ret_hit.w  = hit_w(S);

		sgn_after = ret_hit.vx - ret_hit.w * r_hit;

		//printf("vx = %lf   vy = %lf\n", ret_hit.vx, ret_hit.vy);

		if ( sgn_before * sgn_after <= 0 && mu != 0)
		{
		//	printf("Pereschet\n");
			double c = fabs(current.vx - r_hit * current.w) / 2 * m / mu;
			ret_hit.vx = hit_vx(c);
			ret_hit.w  = hit_w(c);
		}
	//	printf("vx = %lf   vy = %lf\n", ret_hit.vx, ret_hit.vy);
		ret_hit = coord_trans_back(-obj.phi, -obj.vx , -obj.vy , ret_hit);

		return ret_hit;
	}
}