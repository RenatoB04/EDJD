import pandas as pd
import matplotlib.pyplot as plt

file_path = r'C:\Users\Legion\Documents\P01-PE.csv'
data = pd.read_csv(file_path)

variables = ['Depressão', 'Ansiedade', 'Stress', 'Sat. Pessoal']

plt.figure(figsize=(14, 8))

for var in variables:
    plt.hist(data[var], bins=20, alpha=0.5, label=var)

plt.xlabel('Valores')
plt.ylabel('Frequência')
plt.legend()
plt.grid(True)
plt.show()
