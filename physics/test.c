#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <gsl/gsl_integration.h>
#include <pthread.h>
#include <sys/types.h>
#include <sys/ipc.h>
#include <sys/sem.h>
#include <unistd.h>

#define MAIN 1
#define VX 2
#define VY 3

struct Coord
{
    double x , y , phi;
    double vx , vy , w;
};

struct Coord coord = {0 , 0 , 0 , 0 , 0 , 0};
struct Coord next_coord;

// f*g*ro*h*r = factor
double factor = 1;
double r = 1;

double func_vx ( double x, void * params){ return factor * ( coord.vx - coord.w * r * sin(x) )/sqrt( pow((coord.vx - coord.w*r*sin(x)) , 2) + pow((coord.vy + coord.w * r * cos(x)) , 2) ); }
double func_vy ( double x, void * params){ return factor * ( coord.vy + coord.w * r * cos(x) )/sqrt( pow((coord.vx - coord.w*r*sin(x)) , 2) + pow((coord.vy + coord.w * r * cos(x)) , 2) ); }

void* pthread_func_vx(void *sem_id)
{
    // sem init
    struct sembuf sops[2];
    sops[0].sem_num = VX;
    sops[0].sem_op = 0;
    sops[0].sem_flg = SEM_UNDO;
    sops[1].sem_num = VX;
    sops[1].sem_op = 1;
    sops[1].sem_flg = SEM_UNDO;
    sops[2].sem_num = MAIN;
    sops[2].sem_op  = 1;
    sops[2].sem_flg = SEM_UNDO;
    
    
    // integration workflow init
    gsl_integration_workspace * w = gsl_integration_workspace_alloc (1000);
    double result, error;
    double alpha = 1.0;
    
    gsl_function F;
    F.params = &alpha;

    F.function = &func_vx;

    int i = 0;

    for( i = 0 ; i <100 ; i++)
    {
        semop( *(int*)sem_id , &sops[0] , 1);
        gsl_integration_qags (&F, 0, 2*3.1415, 0, 1e-4, 1000,w, &result, &error);
        next_coord.vx = result;
        semop( *(int*)sem_id , &sops[1] , 2);
    }
    
    gsl_integration_workspace_free (w);
}   



void* pthread_func_vy(void *sem_id)
{
    // sem init
    struct sembuf sops[3];
    sops[0].sem_num = VY;
    sops[0].sem_op  = 0;
    sops[0].sem_flg = SEM_UNDO;
    sops[1].sem_num = VY;
    sops[1].sem_op  = 1;
    sops[1].sem_flg = SEM_UNDO;
    sops[2].sem_num = MAIN;
    sops[2].sem_op  = 1;
    sops[2].sem_flg = SEM_UNDO;


    // integration workflow init
    gsl_integration_workspace * w = gsl_integration_workspace_alloc (1000);
    double result, error;
    double alpha = 1.0;
    gsl_function F;
    F.params = &alpha;
    F.function = &func_vy;

    int i = 0;
    for( i = 0 ; i <100; i++)
    {
        semop( *(int*)sem_id , &sops[0] , 1);
        gsl_integration_qags (&F, 0, 2*3.1415, 0, 1e-4, 1000,w, &result, &error);
        next_coord.vy = result;
        semop( *(int*)sem_id , &sops[1] , 2);
    }

    gsl_integration_workspace_free (w);
}   



void* pthread_func_send(void *sem_id)
{
    // sem init
    struct sembuf sops[3];
    sops[0].sem_num = MAIN;
    sops[0].sem_op  = -2;
    sops[0].sem_flg = SEM_UNDO;
    sops[1].sem_num = VX;
    sops[1].sem_op  = -1;
    sops[1].sem_flg = SEM_UNDO;
    sops[2].sem_num = VY;
    sops[2].sem_op  = -1;
    sops[2].sem_flg = SEM_UNDO;

    int i = 0;
    for( i = 0 ; i <100 ; i++)
    {
    // all threads are ready
    semop( *(int*)sem_id , &sops[0] , 1);

    printf ("result vx = % .18f\n", next_coord.vx);
    printf ("result vy = % .18f\n", next_coord.vy);

    coord.vx = next_coord.vx;
    coord.vy = next_coord.vy;
   
    // back to count threads
    semop( *(int*)sem_id , &sops[1] , 2);
    }
}


int main ( int argc, char** argv)
{
    if( argc != 4)
        return -1;
    
    char* end;

    coord.vx = strtod( argv[1] , &end);
    coord.vy = strtod( argv[2] , &end);
    coord.w  = strtod( argv[3] , &end);

    int sem_id = semget( IPC_PRIVATE , 4 , 0777|IPC_CREAT);

    pthread_t pthread_vx , pthread_vy , pthread_w , pthread_send;

    pthread_create( &pthread_vx , NULL , pthread_func_vx , &sem_id);
    pthread_create( &pthread_vy , NULL , pthread_func_vy , &sem_id);

    pthread_create( &pthread_send , NULL , pthread_func_send , &sem_id);

    void* tret;

    pthread_join( pthread_send , &tret);
    pthread_join( pthread_vx , &tret);
    pthread_join( pthread_vy , &tret);

    return 0;
}
