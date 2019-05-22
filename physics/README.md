# movecalcVS.dll
This is a short man how to use ``movecalcVS.dll``.
In [this](https://github.com/project4sem/AeroHockey/tree/master/physics/movecalcVS/x64/Release) path you can find ``movecalcVS.dll`` file.

This are the main functions: 
```
struct Coord movecalc(double _factor_v, double _factor_w, double _dt, double _r, struct Coord _coord);
struct Coord hit(double k, double m, double mu, double r, struct Coord coord, struct Obj obj);
struct Coord boost(double v_max, struct Coord _coord, struct Obj obj);
```
Structs is defined this way:
```
struct Coord
{
    double x , y;
    double vx , vy , w;
};

struct Obj
{
	double vx, vy;
	double phi;
};
```

# movecalc
``_factor_v > 0``, ``_factor_w > 0`` and ``_r > 0`` are factors of intigrations, which are defined by user.
``_dt > 0`` is a time of discritisation of movement.

``movecalc`` function gets ``_coord`` of object and returns ``struct Coord`` - a position of object afer ``_dt``.

# hit
``1 >= k > 0``, ``m > 0``, ``mu > 0``, ``r > 0`` are factors. 
``obj`` cosists of ``vx`` and ``vy`` velocities of ring-hit wall, ``phi`` is an angle of tangent wall in degrees.
This function returns ``struct Coord`` after hit.

# boost
``v_max > 0`` is a factor.
``obj`` cosists of ``phi`` an angle of tangent boost in degrees.
``obj.vx`` and ``obj.vy`` are not used.
This function returns ``struct Coord`` after boost.
