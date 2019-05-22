#include "windows.h"
#include "stdio.h"
#include "stdlib.h"
#include "math.h"
#include <gsl/gsl_integration.h>
#include "sys/types.h"
#include <iostream>
#include "movecalcVS.h"

#define _USE_MATH_DEFINES


extern "C"
{
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

	double hit_vx(double S , double mu , double m , double sgn, struct Coord current)
	{
		double vx;
		if (sgn <= 0)
			vx = current.vx + mu * S / m;
		else
			vx = current.vx - mu * S / m;

		return vx;
	}

	double hit_vy(double k , struct Coord current)
	{
		return - current.vy * k;
	}

	double hit_w(double S , double mu , double m , double r , double sgn , struct Coord current)
	{
		double w;
		if (sgn <= 0)
			w = current.w - mu * S / m / r;
		else
			w = current.w + mu * S / m / r;
		
		return w;
	}

	struct Coord MOVECALC_API hit(double k, double m, double mu , double r, struct Coord coord, struct Obj obj)
	{
		if (k <= 0 || k > 1 || m <= 0 || mu <= 0 || r <= 0)
		{
			std::cerr << "Invalid arguments\n";
			struct Coord inv = { 0 , 0 , 0 , 0 , 0 };
			return inv;
		}

		struct Coord current;
		struct Coord ret;

		obj.phi = M_PI / 2 + obj.phi / 180 * M_PI;

		current = coord_trans(obj.phi , obj.vx , obj.vy , coord);
		
		if (current.vy >= 0)
		{
			obj.phi = obj.phi - M_PI;
			current = coord_trans( obj.phi, obj.vx, obj.vy, coord);
		}

		double S = m * (k + 1) * fabs(current.vy);

		double sgn_before = current.vx - current.w * r;

		ret.x = current.x;
		ret.y = current.y;

		ret.vx = hit_vx(S, mu , m , sgn_before,  current);
		ret.vy = hit_vy(k , current);
		ret.w  = hit_w(S , mu , m , r , sgn_before , current);

		double sgn_after = ret.vx - ret.w * r;

		if ( sgn_before * sgn_after <= 0 && mu != 0)
		{
			double c = fabs(current.vx - r * current.w) / 2 * m / mu;
			ret.vx = hit_vx(c , mu , m , sgn_before , current);
			ret.w  = hit_w(c , mu , m , r , sgn_before , current);
		}

		ret = coord_trans_back(-obj.phi, -obj.vx , -obj.vy , ret);

		return ret;
	}
}