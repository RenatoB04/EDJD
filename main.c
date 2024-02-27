#include <stdio.h>
#include <limits.h>

#define MAX_SIZE 10

int maxSum(int matrix[MAX_SIZE][MAX_SIZE], int n) {
    int sum = 0;
    int selectedRows[MAX_SIZE] = {0};
    int selectedCols[MAX_SIZE] = {0};

    printf("Selected numbers:\n");

    for (int i = 0; i < n; i++) {
        int max = 0;
        int maxIndex = -1;

        for (int j = 0; j < n; j++) {
            if (!selectedRows[i] && !selectedCols[j] && matrix[i][j] > max) {
                max = matrix[i][j];
                maxIndex = j;
            }
        }

        if (maxIndex != -1) {
            printf("%d ", max);
            sum += max;
            selectedRows[i] = 1;
            selectedCols[maxIndex] = 1;
        }
    }

    printf("\n");

    return sum;
}

int main() {
    int matrix[MAX_SIZE][MAX_SIZE] = {
        {7, 53, 183, 439, 863},
        {497, 383, 563, 79, 973},
        {287, 63, 343, 169, 583},
        {627, 343, 773, 959, 943},
        {767, 473, 103, 699, 303}
    };

    int n = sizeof(matrix[0]) / sizeof(matrix[0][0]);

    int result = maxSum(matrix, n);

    printf("Maximum possible sum: %d\n", result);

    return 0;
}