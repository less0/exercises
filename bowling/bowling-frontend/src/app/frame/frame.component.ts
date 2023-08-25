import { Component, Input } from '@angular/core';
import { Frame } from '../model/frame';

@Component({
  selector: 'app-frame',
  templateUrl: './frame.component.html',
  styleUrls: ['./frame.component.less']
})
export class FrameComponent {
  @Input() frame! : Frame;

  public firstRoll() : string
  {
    if(this.frame.rolls!.length > 0)
    {
      var roll = this.frame.rolls![0];
      return roll == 10 ? "X" : roll.toString();
    }

    return "";
  }

  public secondRoll() : string
  {
    if(this.frame.rolls!.length >= 2)
    {
      var firstRoll = this.frame.rolls![0];
      var secondRoll = this.frame.rolls![1];

      if(firstRoll + secondRoll == 10)
      {
        return "/";
      }

      return secondRoll == 10 ? "X" : secondRoll.toString();
    }

    return "";
  }

  public hasThreeRolls() : boolean
  {
    return this.frame.rolls!.length == 3;
  }

  public thirdRoll() : string | undefined
  {
    if(this.hasThreeRolls())
    {
      var roll = this.frame.rolls![2];
      return roll == 10 ? "X" : roll.toString();
    }

    return undefined;
  }
}
