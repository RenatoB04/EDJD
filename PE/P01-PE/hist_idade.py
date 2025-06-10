import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt

file_path = r'C:\Users\Legion\Documents\P01-PE.csv'
data = pd.read_csv(file_path)

plt.figure(figsize=(8, 6))
sns.histplot(data['Idade'], bins=20, kde=True)
plt.xlabel('Idade')
plt.ylabel('Frequência')
plt.grid(True)
plt.show()
