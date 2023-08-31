import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-editable-date-field',
  templateUrl: './editable-date-field.component.html',
  styleUrls: ['./editable-date-field.component.less']
})
export class EditableDateFieldComponent {
  @Input() value!: Date
  @Output() valueChange = new EventEmitter<Date>()

  @Input() editMode!: boolean


  onInputValueChange(value: Date) {
    this.value = value;
    this.valueChange.emit(this.value);
  }
}
