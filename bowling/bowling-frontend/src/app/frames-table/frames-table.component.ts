import { Component, Input } from '@angular/core';
import { Frame } from '../model/frame';

@Component({
  selector: 'app-frames-table',
  templateUrl: './frames-table.component.html',
  styleUrls: ['./frames-table.component.less']
})
export class FramesTableComponent {
  @Input() frames! : Frame[][];
}
