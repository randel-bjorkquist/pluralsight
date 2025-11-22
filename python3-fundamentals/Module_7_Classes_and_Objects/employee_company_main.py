# ---------------------------------------------------------------------------------------------------------------------
# import(s)
# ---------------------------------------------------------------------------------------------------------------------
# NOTE: 'company' refers to the file name 'company.py' and 'Company' refers to the class name 'company.py -> Company'
from company import Company

from hourly_employee      import HourlyEmployee
from salary_employee      import SalaryEmployee
from commission_employee  import CommissionEmployee

# ---------------------------------------------------------------------------------------------------------------------
# Function(s)
# ---------------------------------------------------------------------------------------------------------------------
def main():
  my_company = Company()
  
  # Example:
  # new_employee = Employee('Fred', 'Flintstone')
  # my_company.add_employee(new_employee)

  sarah_hess = SalaryEmployee('Sarah', 'Hess', 50000)
  lee_smith  = HourlyEmployee('Lee','Smith',25, 50)
  bob_brwon  = CommissionEmployee('Bob', 'Brown', 30000, 5, 200)

  my_company.add_employee(sarah_hess)
  my_company.add_employee(lee_smith)
  my_company.add_employee(bob_brwon)

  #print(my_company.employees)
  # OUTPUT: [<employee.Employee object at 0x0000015FA1106BA0>, <employee.Employee object at 0x0000015FA1494A50>, <employee.Employee object at 0x0000015FA1494B90>]

  my_company.display_employees()
  my_company.pay_employees()

# ---------------------------------------------------------------------------------------------------------------------
# Main Program
# ---------------------------------------------------------------------------------------------------------------------
main()


# ---------------------------------------------------------
