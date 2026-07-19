import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { resolveMediaUrl } from '../../utils/media-url';

@Component({
  selector: 'ecom-product-media',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-media.component.html',
  styleUrl: './product-media.component.scss'
})
export class ProductMediaComponent {
  @Input({ required: true }) name!: string;
  @Input() imageUrl: string | null | undefined = null;
  @Input() large = false;

  get src(): string | null {
    return resolveMediaUrl(this.imageUrl);
  }
}
