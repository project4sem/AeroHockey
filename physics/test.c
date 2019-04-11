#include <stdio.h>
#include <math.h>
#include <gsl/gsl_integration.h>

double vx = 3;
double vy = 3;
double w = 2;
double r = 4;

// f*g*ro*h*r = a
double a = 0.1;

double func_vx (double x, void * params)
{
    return a * (vx - w * r * sin(x) )/sqrt( pow((vx - w*r*sin(x)) , 2) + pow((vy + w * r * cos(x)) , 2) );
}


int main (void)
{
    gsl_integration_workspace * w= gsl_integration_workspace_alloc (1000);
    double result, error;
    double alpha = 1.0;
    
    gsl_function F;
    F.function = &func_vx;
    F.params = &alpha;
    gsl_integration_qags (&F, 0, 2*3.1415, 0, 1e-7, 1000,w, &result, &error);
    
    
    printf ("result          = % .18f\n", result);
    printf ("estimated error = % .18f\n", error);
    
    gsl_integration_workspace_free (w);
    
    return 0;
}
