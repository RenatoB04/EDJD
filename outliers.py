import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns

file_path = r'C:\Users\Legion\Documents\P01-PE.csv'
data = pd.read_csv(file_path)

print("Distribuição de Género:")
print(data['Género'].value_counts())

gender_map = {0: 'Masculino', 1: 'Feminino'}
data['Género'] = data['Género'].map(gender_map)

data_filtered = data[['Depressão', 'Género']].dropna()

summary_stats = data_filtered.groupby('Género')['Depressão'].describe()
print("Resumo Estatístico de Depressão por Género:\n", summary_stats)

def identify_outliers(series):
    Q1 = series.quantile(0.25)
    Q3 = series.quantile(0.75)
    IQR = Q3 - Q1
    lower_bound = Q1 - 1.5 * IQR
    upper_bound = Q3 + 1.5 * IQR
    outliers = series[(series < lower_bound) | (series > upper_bound)]
    return outliers

outliers_total = identify_outliers(data_filtered['Depressão'])
outliers_by_gender = data_filtered.groupby('Género')['Depressão'].apply(identify_outliers)

print("Outliers Total:\n", outliers_total.describe())
print("Outliers por Género:\n", outliers_by_gender.describe())

plt.figure(figsize=(12, 6))
sns.boxplot(x='Género', y='Depressão', data=data_filtered)
plt.title('Boxplot de Depressão por Género')
plt.show()

plt.figure(figsize=(12, 6))
sns.histplot(data=data_filtered, x='Depressão', hue='Género', kde=True, element='step')
plt.title('Distribuição de Depressão por Género')
plt.show()