#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <gsl/gsl_integration.h>
#include <pthread.h>
#include <sys/types.h>
#include <unistd.h>
#include <windows.h>

#include "movecalc.h"

#define THREADS_NUM 3


struct Coord coord;
struct Ret_all ret_all;

double factor_v , factor_w , dt , r;

double func_vx ( double x, void * params){ return factor_v * ( coord.vx - coord.w * r * sin(x) )/sqrt( pow((coord.vx - coord.w*r*sin(x)) , 2) + pow((coord.vy + coord.w * r * cos(x)) , 2) ); }
double func_vy ( double x, void * params){ return factor_v * ( coord.vy + coord.w * r * cos(x) )/sqrt( pow((coord.vx - coord.w*r*sin(x)) , 2) + pow((coord.vy + coord.w * r * cos(x)) , 2) ); }
double func_w  ( double x, void * params){ return factor_w * ( r * cos(x) * ( coord.vy + coord.w * r * cos(x) ) - r * sin(x) * (coord.vx - coord.w * r * sin(x)) )/sqrt( pow((coord.vx - coord.w*r*sin(x)) , 2) + pow((coord.vy + coord.w * r * cos(x)) , 2) ); }

static DWORD WINAPI pthread_func_vx(void*)
{
    // integration workflow init
    gsl_integration_workspace * w = gsl_integration_workspace_alloc (1000);
    double result, error;// integration workflow init
    double alpha = 1.0;

    gsl_function F;
    F.params = &alpha;

    F.function = &func_vx;
    // calc
    gsl_integration_qags (&F, 0, 2*3.1415, 0, 1e-4, 1000,w, &result, &error);

    ret_all.factors.cx2 = result/2;
    ret_all.coord.vx = coord.vx - result * dt;

    gsl_integration_workspace_free (w);
    ExitThread(0);
}


static DWORD WINAPI pthread_func_vy(void*)
{
    // integration workflow init
    gsl_integration_workspace * w = gsl_integration_workspace_alloc (1000);
    double result, error;
    double alpha = 1.0;
    gsl_function F;
    F.params = &alpha;
    F.function = &func_vy;
    // calc
    gsl_integration_qags (&F, 0, 2*3.1415, 0, 1e-4, 1000,w, &result, &error);

    ret_all.factors.cy2 = result/2;
    ret_all.coord.vy = coord.vy - result * dt;

    gsl_integration_workspace_free (w);
    ExitThread(0);
}


static DWORD WINAPI pthread_func_w(void*)
{
    // integration workflow init
    gsl_integration_workspace * w = gsl_integration_workspace_alloc (1000);
    double result, error;
    double alpha = 1.0;
    gsl_function F;
    F.params = &alpha;
    F.function = &func_w;
    // calc
    gsl_integration_qags (&F, 0, 2*3.1415, 0, 1e-4, 1000,w, &result, &error);

    ret_all.coord.w = coord.w - result * dt;

    gsl_integration_workspace_free (w);
    ExitThread(0);
}

int DLL_EXPORT her(int x)
{
    return x;
}

struct Ret_all DLL_EXPORT movecalc(const double _factor_v , const double _factor_w , const double _dt , const double _r , const struct Coord _coord)
{
    factor_v = _factor_v;
    factor_w = _factor_w;
    dt = _dt;
    r = _r;

    coord.x  = _coord.x;
    coord.y  = _coord.y;
    coord.vx = _coord.vx;
    coord.vy = _coord.vy;
    coord.w  = _coord.w;

    HANDLE hThreads[THREADS_NUM];

    hThreads[0] = CreateThread(NULL, 0, &pthread_func_vx, NULL, 0, NULL);
    hThreads[1] = CreateThread(NULL, 0, &pthread_func_vy, NULL, 0, NULL);
    hThreads[2] = CreateThread(NULL, 0, &pthread_func_w , NULL, 0, NULL);

    ret_all.factors.cx0 = coord.x;
    ret_all.factors.cx1 = coord.vx;
    ret_all.factors.cy0 = coord.y;
    ret_all.factors.cy1 = coord.vy;

    WaitForMultipleObjects(THREADS_NUM, hThreads, TRUE, INFINITE);

    ret_all.coord.x = ret_all.factors.cx0 + ret_all.factors.cx1 * dt + ret_all.factors.cx2 * dt * dt;
    ret_all.coord.y = ret_all.factors.cy0 + ret_all.factors.cy1 * dt + ret_all.factors.cy2 * dt * dt;

    return ret_all;
}
