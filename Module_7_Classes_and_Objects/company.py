# ---------------------------------------------------------------------------------------------------------------------
# import(s)
# ---------------------------------------------------------------------------------------------------------------------
# NOTE: 'employee' refers to the file name 'employee.py' and 'Employee' refers to the class name 'employee.py -> Employee'
from employee import Employee
from typing   import List # NOTE: needed this to help with explicit typing of self.employees: List[Employee] = []

# ---------------------------------------------------------------------------------------------------------------------
# Class(s)
# ---------------------------------------------------------------------------------------------------------------------
class Company:
  def __init__(self, company_name = None):
    #self.employees = []                 # NOTE: not explicitly typing, does not give access to the Employee intellisense
    self.employees: List[Employee] = [] # Explicit reference to force Pylance to recognize Employee
    self.name = company_name

  @classmethod
  def with_company_name(class_method, company_name):
    instance = class_method()  # Calls __init__ with self only
    instance.name = company_name
    return instance

  def add_employee(self, employee) -> None:
    self.employees.append(employee)
  
  def display_employees(self):
    print('-- Current Employees -------------------------')

    for employee in self.employees:
      print(' -', employee.first_name, employee.last_name)

    print()

  def pay_employees(self):
    print('-- Paying Employees --------------------------')

    for employee in self.employees:
      print('Paycheck for:', employee.first_name, employee.last_name)
      print(f'  Amount: ${employee.calculate_weekly_paycheck():,.2f}')
      print('----------------------------------------------')
  
# ---------------------------------------------------------
   
# ---------------------------------------------------------
