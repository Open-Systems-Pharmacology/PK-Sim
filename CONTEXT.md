# PK-Sim

Physiologically-based pharmacokinetic (PBPK) modelling and simulation. This glossary records terms whose everyday meaning differs from their meaning here, or whose naming has been deliberately chosen. It grows as terms are settled — it is not a complete index of the domain.

## Compound parameters

**Parameter group**:
A named bucket that every compound parameter belongs to, defined in the PK-Sim database rather than in code. Determines which panel a parameter is rendered on and whether it participates in alternatives.
_Avoid_: category, section

**Parameter alternative**:
A named, user-selectable set of values for every parameter in a parameter alternative group — for example two measured lipophilicity values from different labs. Exactly one alternative per group is the default, and a simulation consumes exactly one alternative per group.
_Avoid_: variant, option, parameter set

**Parameter alternative group**:
A parameter group whose parameters are supplied by alternatives instead of directly by the compound. An alternative always carries *every* parameter of its group, which is why a group is split when only some of its parameters should vary.

**Calculated alternative**:
The default alternative of a group whose parameters retain their database formula rather than holding a user-entered constant. Displayed as "Calculated", it shows one derived value per alternative of the group it depends on. Alternatives the user adds alongside it hold constants instead.
_Avoid_: computed alternative, default alternative

## Intestinal solubility

**Advanced solubility parameters**:
The compound parameters describing how bile salt micelles raise a drug's solubility in intestinal lumen fluid. Distinct from the compound's aqueous solubility, which lives in its own group.

**Critical micellar concentration (CMC)**:
The bile salt concentration above which micelles form. Below it, lumen solubility falls back to the aqueous value.
_Note_: misspelled `Crititical Micellar concentration` in the database.

**Bile salt micelle/water partition coefficient**:
How strongly a drug partitions from lumen water into bile salt micelles, held separately for the neutral and ionized species. The neutral coefficient is derived from lipophilicity via two regression constants; the ionized one is derived in turn from the neutral one, discounted by a micelle affinity penalty that depends on the compound's ionization.
_Avoid_: micelle affinity (that names a different, existing solubility parameter)
