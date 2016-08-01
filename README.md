# Biological Signals Acquisition Framework

This framework aims to provide a generic reference C# implementation for biological signals data acquisition, processing, analysis and visualization.

Multiple partial implementations exist (especially in C++) but an Open Source implementation, shareable between Academia and Industry will easy interoperability and provide a robust foundation for professionals, developers and researchers. Each one will focus on her specific target and expertise but with benefits from all the others, effectively reducing time-to-market, interoperability and research costs.

---

This project just started and a lot must be done before an initial release will be available, at least these main areas need contributors and external help:

* Integration with existing hardware.
* Integration with Health platforms (where applicable), for example Microsoft Health.
* Definition of _standard_ interfaces between components to mitigate integration issues.
* Search and analysis of widely-used algorithms and features that should be implemented (please use Issues to suggest any enhancement and to discuss it with the community.)
* Implementation of basic components and algorithms.
* Code review and testing.
* Documentation (this is a extremely important for any Open Source framework) and translations.

---

Why C#? Because it is mature and portable enough to implement this framework, widely known and safer than C++ (especially in Academia where you may want to work more on algorithms than on their implementation.) Obviously not overything can (and should) be done in C# (or other .NET languages) then standard protocols should be used for communication, whenever possible, between components and sub-systems and _interoperability modules_ should be written to ease integration with other environments (I am thinking about C or C++ code for numerical computations and probably Lua or Python as well.)

_Biological signals_ is incredibly vaste topic then project will focus on subsets adding more and more (also with community contribution), for first milestone I'd add basic support for EEG and ECG acquisition then EP/ERP and finally EMG/ENG. After those milestones the project will focus to implement a standard (and basic) visualization framework usable in Academia and extensible by complementors.

More applications are welcome...
