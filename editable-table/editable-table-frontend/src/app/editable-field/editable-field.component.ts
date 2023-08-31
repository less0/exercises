import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-editable-field',
  templateUrl: './editable-field.component.html',
  styleUrls: ['./editable-field.component.less']
})
export class EditableFieldComponent {
  @Input() value!: string;
  @Output() valueChange = new EventEmitter<string>();

  @Input() editMode!: boolean

  // It seemed to be necessary to subscribe to (ngModelChange) and manually emit valueChange. 
  // Simply binding [(ngModel)]="value" did not work in this case.
  onInputValueChange(value: string) {
    this.value = value;
    this.valueChange.emit(this.value);
  }
}
