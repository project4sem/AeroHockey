#include "windows.h"
#include "stdio.h"
#include "stdlib.h"
#include "math.h"
#include <gsl/gsl_integration.h>
#include "sys/types.h"
#include "movecalcVS.h"

struct Coord coord_trans(double phi, struct Coord _coord)
{
	struct Coord _ret = _coord;
	_ret.vx = _coord.vx * cos(phi) + _coord.vy * sin(phi);
	_ret.vy = -_coord.vx * sin(phi) + _coord.vy * cos(phi);
	return _ret;
}

struct Coord MOVECALC_API boost(double v_max, struct Coord _coord , struct Obj obj)
{
	double factor_v = exp((v_max - fabs(_coord.vx)) / v_max) - 1;
	obj.phi = M_PI / 2 + obj.phi / 180 * M_PI;

	Coord current = coord_trans(obj.phi, _coord);
	Coord save = _coord;

	if (_coord.vx > 0)
		_coord.vx += fabs(current.vy * factor_v * sin(obj.phi));
	else 
		_coord.vx -= fabs(current.vy * factor_v * sin(obj.phi));

	if (_coord.vy > 0)
		_coord.vy += fabs(current.vy * factor_v * cos(obj.phi));
	else
		_coord.vy -= fabs(current.vy * factor_v * cos(obj.phi));

	double limit1 = fabs(_coord.vx) / v_max;
	double limit2 = fabs(_coord.vy) / v_max;

	if (limit1 > 1 || limit2 > 1)
	{
		_coord.vx = save.vx;
		_coord.vy = save.vy;
	}

	return _coord;
}