# movecalc .dll
This is a short man how to use movecalc ``.dll``.
In [this](https://github.com/project4sem/AeroHockey/tree/master/physics/movecalcVS/x64/Release) path you can find ``movecalcVS.dll`` file.

This is the main function: 
```
struct Ret_all DLL_EXPORT movecalc(double _factor_v , double _factor_w , double _dt , double _r , struct Coord _coord)
```
, structs are defined this way:
```
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
```
Also, ``_factor_v``, ``_factor_w`` and ``_r`` are factors of intigrations, which are defined by user.
``_dt`` is a time of discritisation of movement.

This function gets ``_coord`` of object and returns ``struct Ret_all``, which consists of ``struct Coord`` - a position of object afer ``_dt`` and ``struct Ret_factors`` - describes movement at this time:
```
x = cx0 + cx1*t + cx2*t^2
y = cy0 + cy1*t + cy2*t^2
```
