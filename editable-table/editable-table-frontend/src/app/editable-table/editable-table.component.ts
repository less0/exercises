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
  departments : Department[] | undefined
  persons : Person[] | undefined

  constructor(private httpClient : HttpClient) {
  }

  ngOnInit(): void {
    this.httpClient.get<Person[]>(`${Configuration.apiUrl}/persons`)
      .subscribe(persons => this.persons = persons);
    this.httpClient.get<Department[]>(`${Configuration.apiUrl}/departments`)
      .subscribe(departments => this.departments = departments);
  }
}
