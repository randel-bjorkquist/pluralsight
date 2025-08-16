# ---------------------------------------------------------------------------------------------------------------------
# Import(s)
# ---------------------------------------------------------------------------------------------------------------------
from salary_employee import SalaryEmployee

# ---------------------------------------------------------------------------------------------------------------------
# Class(s)
# ---------------------------------------------------------------------------------------------------------------------
class CommissionEmployee(SalaryEmployee):
  def __init__(self, first_name, last_name, salary, sales_number, commission_rate):
    super().__init__(first_name, last_name, salary)

    self.sales_number    = sales_number
    self.commission_rate = commission_rate

  def calculate_weekly_paycheck(self):
    weekly_salary    = super().calculate_weekly_paycheck()
    total_commission = self.sales_number * self.commission_rate
    
    return weekly_salary + total_commission
   
# ---------------------------------------------------------
   
# ---------------------------------------------------------
