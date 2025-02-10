# Mental Hospital Application

### Introduction
This project is part of a university course, where the main objective was to design and implement a local storing system structure from scratch. The system is a comprehensive medical platform designed to streamline workflow for hospital employees and provide easy access to various types of patient and facility information.

Read full documentation in [`Mental_Hospital_Documentation.pdf`](https://github.com/Daryna-Elena-Yan/mental-hospital/blob/main/Mental_Hospital_Documentation.pdf)
## Project Scope

The system facilitates the management of:
- **Patient Information**: Including personal data, diagnoses, anamnesis, and treatment history.
- **Room Assignments**: Handling patient placements with historical records.
- **Employee Management**: Storing details about nurses and therapists, their salaries, overtime, and supervisory roles.
- **Medical Appointments & Diagnoses**: Keeping records of therapy sessions, diagnoses, and treatment plans.
- **Prescriptions & Medications**: Managing prescriptions associated with patient appointments.
- **Equipment Tracking**: Assigning and monitoring medical equipment in rooms

## Features & Design Decisions
- **Local Storage System**: Designed and implemented a custom structure for data persistence.
- **Factory & Validator Classes**: Ensured clean data entry and data integrity.
- **Generic Storage Class**: Utilized generic programming principles to efficiently manage different types of objects.
- **Bidirectional Association Management**: Implemented using reflection to ensure entity relationships remain consistent.
- **AssociationCollection & AssociationDictionary**: Custom implementations for handling complex relationships dynamically, ensuring efficient entity retrieval and storage.
- **Dynamic Object Restoration**: Implemented RestoreObjects() method in custom association classes to ensure object relationships are properly re-established after deserialization.
- **Composition & Reflexive Associations**: Ensured robust entity lifecycle management, with automatic cleanup of dependent objects and self-referencing relationships.
- **Custom Serialization Mechanism**: Designed an efficient JSON serialization strategy, incorporating JsonIgnore annotations to prevent infinite loops while maintaining necessary entity links.

## Installation
To run the system locally:
- Clone the repository:
  
  ` git clone <repository_url> `
- Install required dependencies
- Build and run the project in a compatible .NET environment

## Authors
Developed by [Daryna Vlasiuk](https://github.com/darinavlasiuk), [Elena Ulasau](https://github.com/elenaulasau), [Yan-Pavel Hantsavich](https://github.com/yan-gans)
