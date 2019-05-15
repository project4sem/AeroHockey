# movecalc .dll
This is a short man how to use movecalc ``.dll``.
In [this](https://github.com/project4sem/AeroHockey/tree/master/physics/win/dll/movecalc) path you can find ``movecalc.h`` file,
in [this](https://github.com/project4sem/AeroHockey/tree/master/physics/win/dll/movecalc/bin/Debug) path you can find ``movecalc.dll`` file.

This is the main function: 
```
struct Ret_all DLL_EXPORT movecalc(const double _factor_v , const double _factor_w , const double _dt , const double _r , const struct Coord _coord)
```
, where structs are defined this way:
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

This fumction gets ``_coord`` of object and returns ``struct Ret_all``, which consists of ``struct Coord`` - a position of object afer ``_dt`` and ``struct Ret_factors``
- describes movement at this time:
```
x = cx0 + cx1*t + cx2*t^2
y = cy0 + cy1*t + cy2*t^2
```
