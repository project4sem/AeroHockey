#include "windows.h"
#include "stdio.h"
#include "stdlib.h"
#include "math.h"
#include <gsl/gsl_integration.h>
#include "sys/types.h"
#include <iostream>
#include "movecalcVS.h"

#define THREADS_NUM 3

extern "C"
{
	struct Coord coord;
	struct Coord ret;

	double factor_v, factor_w, dt, r;

	double func_vx(double x, void* params) { return factor_v * (coord.vx - coord.w * r * sin(x)) / sqrt(pow((coord.vx - coord.w * r * sin(x)), 2) + pow((coord.vy + coord.w * r * cos(x)), 2)); }
	double func_vy(double x, void* params) { return factor_v * (coord.vy + coord.w * r * cos(x)) / sqrt(pow((coord.vx - coord.w * r * sin(x)), 2) + pow((coord.vy + coord.w * r * cos(x)), 2)); }
	double func_w(double x, void* params) { return factor_w * (r * cos(x) * (coord.vy + coord.w * r * cos(x)) - r * sin(x) * (coord.vx - coord.w * r * sin(x))) / sqrt(pow((coord.vx - coord.w * r * sin(x)), 2) + pow((coord.vy + coord.w * r * cos(x)), 2)); }

	static DWORD pthread_func_vx(void*)
	{
		// integration workflow init
		gsl_integration_workspace* w = gsl_integration_workspace_alloc(1000);
		double result, error;// integration workflow init
		double alpha = 1.0;

		gsl_function F;
		F.params = &alpha;

		F.function = &func_vx;
		// calc
		gsl_integration_qags(&F, 0, 2 * 3.1415, 0, 1e-4, 1000, w, &result, &error);

		ret.vx = coord.vx - result * dt;

		if (ret.vx * coord.vx < 0)
		{
			ret.vx = 0;
		}

		gsl_integration_workspace_free(w);
		ExitThread(0);
	}


	static DWORD pthread_func_vy(void*)
	{
		// integration workflow init
		gsl_integration_workspace* w = gsl_integration_workspace_alloc(1000);
		double result, error;
		double alpha = 1.0;
		gsl_function F;
		F.params = &alpha;
		F.function = &func_vy;
		// calc
		gsl_integration_qags(&F, 0, 2 * 3.1415, 0, 1e-4, 1000, w, &result, &error);

		ret.vy = coord.vy - result * dt;

		if (ret.vy * coord.vy < 0)
		{
			ret.vy = 0;
		}

		ret.x = coord.x + ret.vx * dt + result * dt * dt / 2;

		gsl_integration_workspace_free(w);
		ExitThread(0);
	}


	static DWORD pthread_func_w(void*)
	{
		// integration workflow init
		gsl_integration_workspace* w = gsl_integration_workspace_alloc(1000);
		double result, error;
		double alpha = 1.0;
		gsl_function F;
		F.params = &alpha;
		F.function = &func_w;
		// calc
		gsl_integration_qags(&F, 0, 2 * 3.1415, 0, 1e-4, 1000, w, &result, &error);

		ret.w = coord.w - result * dt;

		if (ret.w * coord.w <= 0)
		{
			ret.w = 0;
		}

		ret.y = coord.y + ret.vy * dt + result * dt * dt / 2;

		gsl_integration_workspace_free(w);
		ExitThread(0);
	}

	struct Coord MOVECALC_API movecalc(double _factor_v, double _factor_w, double _dt, double _r, struct Coord _coord)
	{
		if (_factor_v <= 0 || _factor_w <= 0 || _dt <= 0 || _r <= 0)
		{
			std::cerr << "Invalid arguments\n";
			struct Coord inv = { 0 , 0 , 0 , 0 , 0 };
			return inv;
		}
			
		if (_coord.vx == 0 && _coord.vy == 0 && _coord.w == 0)
		{
			ret = _coord;
			return ret;
		}

		factor_v = _factor_v;
		factor_w = _factor_w;
		dt = _dt;
		r = _r;

		coord = _coord;

		HANDLE hThreads[THREADS_NUM];

		hThreads[0] = CreateThread(NULL, 0, &pthread_func_vx, NULL, 0, NULL);
		hThreads[1] = CreateThread(NULL, 0, &pthread_func_vy, NULL, 0, NULL);
		hThreads[2] = CreateThread(NULL, 0, &pthread_func_w, NULL, 0, NULL);

		WaitForMultipleObjects(THREADS_NUM, hThreads, TRUE, INFINITE);

		return ret;
	}
}
