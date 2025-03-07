# T4 - temp
# T6 - temp outside
# 0.01 - people in room
# 0 - previous command
# кондиционер - макс - 9 кубометров - 17градусов
# кондиционер - средн - 4.5 кубометра


import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as seabornInstance
from sklearn.model_selection import train_test_split
from sklearn.linear_model import LinearRegression
from sklearn import linear_model
from sklearn import metrics
import sys


dataset = pd.read_csv('C:\\Projects\\iot\\HVAC\\Decisions Layer\\data\\temprature-data\\combined.csv')
dataset['people'] = 0
dataset['command'] = 0

X = dataset[['T4','T_out']].head(-1).values  # head().values
y = dataset['T4'].loc[1:].values  # loc[0:349,].values


step = 657

regressor = linear_model.SGDRegressor(max_iter=1000, tol=1e-3)

for i in range(0,30):
    frm = i * step
    to = frm + step
    X_train, X_test, y_train, y_test = train_test_split(X[0:to], y[0:to], test_size=0.1, random_state=0)
    regressor.fit(X_train, y_train)
    y_pred = regressor.predict(X_test)
    print(metrics.mean_absolute_error(y_test, y_pred))
    # df = pd.DataFrame({'Actual': y_test, 'Predicted': y_pred})
    # df1 = df.head(25)

    # df1.plot(kind='bar', figsize=(10, 8))
    # plt.grid(which='major', linestyle='-', linewidth='0.5', color='green')
    # plt.grid(which='minor', linestyle=':', linewidth='0.5', color='black')
    # plt.show()






# coeff_df = pd.DataFrame(regressor.coef_, ['T6', 'people', 'command'], columns=['Coefficient'])
# print('coeff_df')
# print(coeff_df)

# y_pred = regressor.predict(X_test)
# df = pd.DataFrame({'Actual': y_test, 'Predicted': y_pred})
# df1 = df.head(25)

# df1.plot(kind='bar', figsize=(10, 8))
# plt.grid(which='major', linestyle='-', linewidth='0.5', color='green')
# plt.grid(which='minor', linestyle=':', linewidth='0.5', color='black')
# plt.show()

# print('Mean Absolute Error:', metrics.mean_absolute_error(y_test, y_pred))
# print('Mean Squared Error:', metrics.mean_squared_error(y_test, y_pred))
# print('Root Mean Squared Error:', np.sqrt(metrics.mean_squared_error(y_test, y_pred)))

# print('----------------------------')
# print('Command 1')

# dataset['T4'].add(-1)
# dataset['command'] = 1
# X = dataset[['T4', 'T6', 'people', 'command']].head(-1).values
# y = dataset['T4'].loc[1:,].values

# X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=0)
# regressor.partial_fit(X_train, y_train)

# coeff_df = pd.DataFrame(regressor.coef_, ['T4', 'T6', 'people', 'command'], columns=['Coefficient'])  
# print('coeff_df')
# print(coeff_df)
# y_pred = regressor.predict(X_test)
# print('Mean Absolute Error:', metrics.mean_absolute_error(y_test, y_pred))  
# print('Mean Squared Error:', metrics.mean_squared_error(y_test, y_pred))  
# print('Root Mean Squared Error:', np.sqrt(metrics.mean_squared_error(y_test, y_pred)))

# print('----------------------------')
# print('Command 2')

# dataset['T4'].add(-2)
# dataset['command'] = 2
# X = dataset[['T4', 'T6', 'people', 'command']].head(-1).values
# y = dataset['T4'].loc[1:,].values

# X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=0)
# regressor.partial_fit(X_train, y_train)

# coeff_df = pd.DataFrame(regressor.coef_, ['T4', 'T6', 'people', 'command'], columns=['Coefficient'])  
# print('coeff_df')
# print(coeff_df)
# y_pred = regressor.predict(X_test)
# print('Mean Absolute Error:', metrics.mean_absolute_error(y_test, y_pred))  
# print('Mean Squared Error:', metrics.mean_squared_error(y_test, y_pred))  
# print('Root Mean Squared Error:', np.sqrt(metrics.mean_squared_error(y_test, y_pred)))  

# [...]
# training_parameters_values = ''
# fact_temperature_values = ''
# fact_temperature= ''
# parameters_values=''
# def hasMoreParameters():
#     return True

# # начальное обучение модели на исторических собранных данных
# regressor = linear_model.SGDRegressor(max_iter=1000, tol=1e-3)
# regressor.fit(training_parameters_values, fact_temperature_values)
# #
# [...]
# while hasMoreParameters():
#     [...]
#     # обновление коэффициентов модели
#     regressor.partial_fit(temperature_prediction, fact_temperature)
#     [...]
#     # предсказание значения темепературы через 10 минут
#     temperature_prediction = regressor.predict(parameters_values)
# [...]



# # начальное обучение модели на исторических собранных данных
# regressor = linear_model.SGDRegressor(max_iter=1000, tol=1e-3)
# regressor.fit(training_parameters_values, fact_temperature_values)

# [...]
# while hasMoreParameters():
#     [...]
#     # обновление коэффициентов модели
#     regressor.partial_fit(temperature_prediction, fact_temperature)
#     [...]
#     # предсказание значения темепературы через 10 минут
#     temperature_prediction = regressor.predict(parameters_values)
# [...]