# ---------------------------------------------------------------------------------------------------------------------
# Import(s)
# ---------------------------------------------------------------------------------------------------------------------
from employee import Employee

# ---------------------------------------------------------------------------------------------------------------------
# Class(s)
# ---------------------------------------------------------------------------------------------------------------------
class HourlyEmployee(Employee):
  def __init__(self, first_name, last_name, weekly_hours, hourly_rate):
    super().__init__(first_name, last_name)
    
    self.hourly_rate = hourly_rate
    self.weekly_hours = weekly_hours
  
  def calculate_weekly_paycheck(self):
    return self.hourly_rate * self.weekly_hours
  
  #def calculate_weekly_paycheck(self, weekly_hours = 40):
  #  return self.hourly_rate * weekly_hours
   
# ---------------------------------------------------------
   
# ---------------------------------------------------------
