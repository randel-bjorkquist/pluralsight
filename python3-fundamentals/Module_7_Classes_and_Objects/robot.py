# ---------------------------------------------------------------------------------------------------------------------
# Class(s)
# ---------------------------------------------------------------------------------------------------------------------
class Robot:
  def __init__(self, name):
    self.name = name
    self.position = [0, 0]
    print('My name is:', self.name)

  def walk_to_x(self, x):
    self.position[0] = self.position[0] + x
    print('My position is:', self.position)

  def walk_to_y(self, y):
    self.position[1] = self.position[1] + y
    print('My position is:', self.position)

  def walk_to_xy(self, x, y):
    self.position[0] = self.position[0] + x
    self.position[1] = self.position[1] + y
    print('New position:', self.position)

  def eat(self):
    print("I'm hungry")

# ---------------------------------------------------------
