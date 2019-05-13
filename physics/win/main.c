#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <gsl/gsl_integration.h>
#include <pthread.h>
#include <sys/types.h>
#include <unistd.h>
#include <windows.h>

#define THREADS_NUM 3
#define DT 1

struct Coord
{
    double x , y;
    double vx , vy , w;
};

struct Ret_factors
{
    double cx0 , cx1 , cx2;
    double cy0 , cy1 , cy2;
};

struct Ret_all
{
    struct Coord coord;
    struct Ret_factors factors;
};

struct Coord coord;
struct Ret_all ret_all;


// f*g*ro*h*r = factor_v
double factor_v = 1;
double factor_w = 2;
double r = 1;

double func_vx ( double x, void * params){ return factor_v * ( coord.vx - coord.w * r * sin(x) )/sqrt( pow((coord.vx - coord.w*r*sin(x)) , 2) + pow((coord.vy + coord.w * r * cos(x)) , 2) ); }
double func_vy ( double x, void * params){ return factor_v * ( coord.vy + coord.w * r * cos(x) )/sqrt( pow((coord.vx - coord.w*r*sin(x)) , 2) + pow((coord.vy + coord.w * r * cos(x)) , 2) ); }
double func_w  ( double x, void * params){ return factor_w * ( r * cos(x) * ( coord.vy + coord.w * r * cos(x) ) - r * sin(x) * (coord.vx - coord.w * r * sin(x)) )/sqrt( pow((coord.vx - coord.w*r*sin(x)) , 2) + pow((coord.vy + coord.w * r * cos(x)) , 2) ); }

void* pthread_func_vx()
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
    ret_all.coord.vx = coord.vx + result * DT;

    gsl_integration_workspace_free (w);
    ExitThread(0);
}


void* pthread_func_vy()
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
    ret_all.coord.vy = coord.vy + result * DT;

    gsl_integration_workspace_free (w);
    ExitThread(0);
}


void* pthread_func_w()
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

    ret_all.coord.w = coord.w + result * DT;

    gsl_integration_workspace_free (w);
    ExitThread(0);
}


int main ()
{
    coord.vx = 1;
    coord.vy = 2;
    coord.w  = 3;

    HANDLE hThreads[THREADS_NUM];

    hThreads[0] = CreateThread(NULL, 0, &pthread_func_vx, NULL, 0, NULL);
    hThreads[1] = CreateThread(NULL, 0, &pthread_func_vy, NULL, 0, NULL);
    hThreads[2] = CreateThread(NULL, 0, &pthread_func_w , NULL, 0, NULL);

    ret_all.factors.cx0 = coord.x;
    ret_all.factors.cx1 = coord.vx;
    ret_all.factors.cy0 = coord.y;
    ret_all.factors.cy1 = coord.vy;

    WaitForMultipleObjects(THREADS_NUM, hThreads, TRUE, INFINITE);

    ret_all.coord.x = ret_all.factors.cx0 + ret_all.factors.cx1 * DT + ret_all.factors.cx2 * DT * DT;
    ret_all.coord.y = ret_all.factors.cy0 + ret_all.factors.cy1 * DT + ret_all.factors.cy2 * DT * DT;

    return 0;
}
