#ifndef __MOVECALC_H__
#define __MOVECALC_H__

/*  To use this exported function of dll, include this header
 *  in your project.
 */

#ifdef BUILD_DLL
    #define DLL_EXPORT __declspec(dllexport)
#else
    #define DLL_EXPORT __declspec(dllimport)
#endif


#ifdef __cplusplus
extern "C"
{
#endif

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

struct Ret_all DLL_EXPORT movecalc(const double _factor_v , const double _factor_w , const double _dt , const double _r , const struct Coord _coord);
int DLL_EXPORT her(int x);
#ifdef __cplusplus
}
#endif

#endif // __MOVECALC_H__
