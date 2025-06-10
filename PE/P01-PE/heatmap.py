import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt

file_path = r'C:\Users\Legion\Documents\P01-PE.csv'
data = pd.read_csv(file_path)

corr_filtered = data.drop(columns=['Género', 'Tempo S. Suficiente']).corr()

plt.figure(figsize=(10, 8))
sns.heatmap(corr_filtered, annot=True, cmap='coolwarm', cbar=True)
plt.show()
