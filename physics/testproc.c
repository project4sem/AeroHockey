#include <stdio.h>
int main () {
	int i = 0;
	while(1){
		printf("%d\n",i);
		if(i >= 100)
			break;
		i++;
	}
	return 0;
}