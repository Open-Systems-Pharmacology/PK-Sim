# Welcome to PK-Sim
![pksim](https://cloud.githubusercontent.com/assets/1041237/22438535/5b908010-e6fa-11e6-802b-a79992b54188.png)

PK-Sim® is a comprehensive software tool for whole-body physiologically based pharmacokinetic modeling. 
It enables rapid access to all relevant anatomical and physiological parameters for humans and 
the most common pre-clinical animal models (mouse, rat, minipig, dog, and monkey) 
that are contained in the integrated database. Moreover, access to different PBPK calculation methods 
to allow for fast and efficient model building and parameterization is provided. 
Relevant generic passive processes, such as distribution through blood flow 
as well as specific active processes such as metabolization by a certain enzyme 
are automatically taken into account by PK-Sim®. 
Like most PBPK modeling tools, PK-Sim® is designed for use by non-modeling experts 
and only allows for minor structural model modifications. 
Unlike most PBPK modeling tools though, PK-Sim® offers different model structures to choose from, 
e.g. to account for important differences between small and large molecules. 
More importantly, PK-Sim® is fully compatible with the expert modeling software tool MoBi®, 
thereby allowing full access to all model details including the option for extensive model modifications and extensions. 
This way customized systems pharmacology models may be set up to deal with the challenges of modern drug research and development.

PK-Sim® uses building blocks that are grouped into Individuals, Populations, Compounds, Formulations, 
Administration Protocols, Events, and Observed Data. Building blocks from these groups are combined to produce a model. 
The advantage of building blocks is that they can be reused. 
For example, after having established a model for a drug after single dose intravenous administration to an animal species, 
just substitute the individual by a suitably parameterized virtual human population and obtain a first in man simulation model. 
Further substitute the formulation, to obtain a controlled-release per oral simulation model, substitute the protocol 
to obtain a multiple dose simulation model, or substitute the compound to obtain a simulation model for another drug.


## Code Status
[![Build status](https://ci.appveyor.com/api/projects/status/65aa66s8aj2tcp45/branch/master?svg=true&passingText=master%20-%20passing)](https://ci.appveyor.com/project/open-systems-pharmacology-ci/pk-sim/branch/master)
[![Coverage Status](https://coveralls.io/repos/github/rwmcintosh/PK-Sim/badge.svg?branch=master)](https://coveralls.io/github/rwmcintosh/PK-Sim?branch=master)

## Code of conduct
Everyone interacting in the Open Systems Pharmacology community (codebases, issue trackers, chat rooms, mailing lists etc...) is expected to follow the Open Systems Pharmacology [code of conduct](https://github.com/Open-Systems-Pharmacology/Suite/blob/master/CODE_OF_CONDUCT.md).

## Contribution
We encourage contribution to the Open Systems Pharmacology community. Before getting started please read the [contribution guidelines](https://github.com/Open-Systems-Pharmacology/Suite/blob/master/CONTRIBUTING.md). If you are contributing code, please be familiar with the [coding standard](https://github.com/Open-Systems-Pharmacology/Suite/blob/master/CODING_STANDARD.md).

## License
PK-Sim is released under the [GPLv2 License](LICENSE).

All trademarks within this document belong to their legitimate owners.