# Modelling spread of disease
# https://www.maa.org/press/periodicals/loci/joma/the-sir-model-for-spread-of-disease-the-differential-equation-model
import random
import numpy as np
import matplotlib.pyplot as plt
from random import uniform

def w_choice(seq):
    """
    Takes an input on the form of [('A', 30), ('B', 40), ('C', 30)] and returns
    one of the items based on the probality from the second value in each tuple.
    Eg. B will get chosen 40% of the time while A and C will get chosen 30% of
    the time.
    """
    total_prob = sum(item[1] for item in seq)
    chosen = random.uniform(0, total_prob)
    cumulative = 0
    for item, probality in seq:
        cumulative += probality
        if cumulative > chosen:
            return item

class Disease:
    """
    Uses the SIR model to model the spread of a certain disease.

    At each time t, s(t) + i(t) + r(t) = 1
    """

    def __init__(self, N, b, k, susceptible, infected, recovered):
        """
        params:
            N = The total population
            b = The average _FIXED_ number of contacts per day per person.
            k = The average _FIXED_ fraction of infected individuals that will recover per day.
            susceptible = The initial amount of susceptible individuals
            infected = The initial amount of infected individuals
            recovered = The initial amount of recovered individuals
        """
        self.time = 0
        self._N = N
        self._b = b
        self._k = k
        self._susceptible = susceptible
        self._infected = infected
        self._recovered = recovered

    def N(self):
        return self._N

    def vaccinate(self, amount):
        """
        Vaccinates the specified amount of the population.
        """
        self.susceptible(self.susceptible() - amount)

    def susceptible(self, v=None):
        """
        The number of susceptible individuals.
        S = S(t)

        No one is added to this group due to births and immigrations begin ignored.
        An individual will leave this group once infected.
        """
        if v is None:
            return self._susceptible
        self._susceptible = v

    def susceptible_dt(self):
        """
        The rate of change for susceptible individuals.
        dS/dt = -b*s(t)*I(t)
        """
        return -1 * self._b * self.susceptible_fraction() * self.infected()

    def infected(self, v=None):
        """
        The number of infected individuals.
        I = I(t)

        Each infected individual infects b * s(t) new individuals.
        """
        if v is None:
            return self._infected
        self._infected = v

    def recovered(self, v=None):
        """
        The number of recovered individuals.
        R = R(t)
        """
        if v is None:
            return self._recovered
        self._recovered = v

    def susceptible_fraction(self):
        """
        The susceptible fraction of the population.

        s(t)
        """
        return self.fraction(self.susceptible())

    def susceptible_fraction_dt(self):
        """
        The rate of change for the susceptible fraction.
        ds/dt = -b*s(t)*i(t)
        """
        return -1 * self._b * self.susceptible_fraction() * self.infected_fraction()

    def recovered_fraction(self):
        """
        The recovered fraction of the population.

        r(t)
        """
        return self.fraction(self.recovered())

    def recovered_fraction_dt(self):
        """
        The rate of change for the recovered fraction of the population.
        dr/dt = k*i(t)
        """
        return self._k * self.infected_fraction()

    def infected_fraction(self):
        """
        The infected fraction of the population.

        i(t)
        """
        return self.fraction(self.infected())

    def infected_fraction_dt(self):
        """
        The rate of change for the infected fraction of the population.
        di/dt = b * s(t) * i(t) - k * i(t)
        di/dt = -ds/dt - dr/dt
        """
        return -1 * (self.susceptible_fraction_dt() + self.recovered_fraction_dt())

    def fraction(self, nr):
        """
        Returns the fraction of the total population.
        nr / N
        """
        return nr / self._N

    def number(self, fraction):
        """
        Returns the number of the total population
        fraction * N
        """
        return fraction * self._N

    def step(self):
        """
        Steps forward one unit in time.
        """
        i_dt = self.number(self.infected_fraction_dt())
        r_dt = self.number(self.recovered_fraction_dt())
        s_dt = self.number(self.susceptible_fraction_dt())
        self.susceptible(self.susceptible() + s_dt)
        self.recovered(self.recovered() + r_dt)
        self.infected(self.infected() + i_dt)

class City:
    """
    City.
    """

    def __init__(self, name, position, N, b, k, susceptible, infected, recovered):
        """
        params:
            name = The name of the city.
            population = The population of the city
            position = The position of the city
        """
        self._position = position
        self._name = name
        self._disease = Disease(N=N, b=b, k=k, susceptible=susceptible, infected=infected, recovered=recovered)
        self.Y_susceptible = []
        self.Y_infected = []
        self.Y_recovered = []

    def population(self):
        """
        Returns the population of the city.
        """
        return self.disease().N()

    def position(self):
        """
        Returns the position of the city.
        """
        return self.position

    def disease(self):
        return self._disease

    def vaccinate(self, n):
        """
        Vaccinates n people of the population.
        """
        self.disease().vaccinate(n)

    def name(self):
        return self._name

    def step(self, t, _print=False):
        """
        Steps forward one step in time.
        params:
            t = The current timestamp (Used when printing).
            _print = If output should be printed to the console
        """
        d = self.disease()
        self.log(d, t, _print)
        d.step()

    def log(self, d, t, _print):
        """
        Logs the current state of the disease for later plotting and/or dumping
        to the console.
        """
        self.Y_susceptible.append(d.susceptible_fraction())
        self.Y_infected.append(d.infected_fraction())
        self.Y_recovered.append(d.recovered_fraction())
        if _print:
            print("S("+str(t)+")", "%0.2f" % d.susceptible(), "\ts("+str(t)+")=", "%0.2f" % d.susceptible_fraction())
            print("I("+str(t)+")", "%0.2f" % d.infected(), "\ti("+str(t)+")=", "%0.2f" % d.infected_fraction())
            print("R("+str(t)+")", "%0.2f" % d.recovered(), "\tr("+str(t)+")=", "%0.2f" % d.recovered_fraction())
            print()

    def plot(self):
        """
        Plots the progress of the diesease for this City.
        """
        _ys, = plt.plot(self.Y_susceptible, label='s(t)')
        _yi, = plt.plot(self.Y_infected, label="i(t)")
        _yr, = plt.plot(self.Y_recovered, label="r(t)")
        plt.legend(handles=[_ys, _yi, _yr])
        plt.title(self.name())
        plt.show()

def example_graph():
    """
    Example graph that is the same as the one found at the link:
    https://www.maa.org/press/periodicals/loci/joma/the-sir-model-for-spread-of-disease-the-differential-equation-model
    """
    c = City(name="Sweden", position=(0,0), N=7900000, b=1/2, k=1/3, susceptible=7900000, infected=10, recovered=0)

    for t in range(140):
        c.step(t)

    c.plot()

def example_cities():
    b = 1/2
    k = 1/3
    cities = []
    for i in range(4):
        name = "City " + str(i)
        pos = (0,0)
        N = 7900000
        susceptible = N
        infected = 0
        recovered = 0
        c = City(name=name, position=pos, N=N, b=b, k=k, susceptible=susceptible, infected=infected, recovered=recovered)

example_cities()
