import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../application/services/product.service';
import { Product } from '../../../shared/models/product.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  selectedProduct: Product | null = null;
  selectedQuantity: number = 1;
  showModal: boolean = false;

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.products = this.productService.getProducts();
  }

  openProductModal(product: Product): void {
    this.selectedProduct = product;
    this.selectedQuantity = 1;
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.selectedProduct = null;
    this.selectedQuantity = 1;
  }

  addToCart(): void {
    if (this.selectedProduct) {
      this.productService.addToCart(this.selectedProduct, this.selectedQuantity);
      this.closeModal();
    }
  }

  increaseQuantity(): void {
    if (this.selectedProduct && this.selectedQuantity < this.selectedProduct.quantity) {
      this.selectedQuantity++;
    }
  }

  decreaseQuantity(): void {
    if (this.selectedQuantity > 1) {
      this.selectedQuantity--;
    }
  }
}
