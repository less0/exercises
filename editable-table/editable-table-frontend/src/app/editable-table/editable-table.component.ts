import { Component } from '@angular/core';
import { Configuration } from '../configuration';
import { Department } from '../model/department';
import { Person } from '../model/person';
import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';

@Component({
  selector: 'app-editable-table',
  templateUrl: './editable-table.component.html',
  styleUrls: ['./editable-table.component.less']
})
export class EditableTableComponent implements OnInit {
  columnsToDisplay = ['firstName', 'lastName', 'dateOfBirth', 'department', 'actions'];
  departments: Department[] | undefined
  persons: Person[] | undefined

  constructor(private httpClient: HttpClient) {
  }

  ngOnInit(): void {
    this.httpClient.get<Person[]>(`${Configuration.apiUrl}/persons`)
      .subscribe(persons => {
        this.persons = persons;
        this.httpClient.get<Department[]>(`${Configuration.apiUrl}/departments`)
          .subscribe(departments => {
            this.departments = departments;

            for (let personIndex = 0; personIndex < this.persons!.length; personIndex++) {
              var department = this.persons![personIndex].department;
              var departmentToSet = departments.find(d => d.name == department.name);
              if(departmentToSet != undefined)
              {
                this.persons![personIndex].department = departmentToSet;
              }
            }
          });
      });

  }

  save(person: Person): void {
    let personLink = person.links!["self"];

    if (personLink == undefined) {
      return;
    }

    let personToSend: Person =
    {
      id: person.id,
      firstName: person.firstName,
      lastName: person.lastName,
      dateOfBirth: person.dateOfBirth,
      department: person.department,
      $$isInEditMode: false,
      links: undefined
    };

    this.httpClient.put(`${Configuration.apiUrl}${personLink}`, personToSend)
      .subscribe(_ => {
        person.$$isInEditMode = false
      });
  }
}
