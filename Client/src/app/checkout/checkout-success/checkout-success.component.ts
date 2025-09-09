import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IOrder } from 'src/app/shared/models/order';
import { OrdersService } from 'src/app/orders/orders.service';

@Component({
  selector: 'app-checkout-success',
  templateUrl: './checkout-success.component.html',
  styleUrls: ['./checkout-success.component.scss']
})
export class CheckoutSuccessComponent implements OnInit, OnDestroy {
  order: IOrder;
  pollingIntervalId: any;
  pollingStarted = false;

  constructor(private router: Router, private ordersService: OrdersService) {
    const navigation = this.router.getCurrentNavigation();
    const state = navigation && navigation.extras && navigation.extras.state;
    if (state) {
      this.order = state as IOrder;
    }
   }

  ngOnInit(): void {
    this.startPollingIfNeeded();
  }

  ngOnDestroy(): void {
    if (this.pollingIntervalId) {
      clearInterval(this.pollingIntervalId);
    }
  }

  private startPollingIfNeeded() {
    if (!this.order || !this.order.id || this.order.status !== 'Pending' || this.pollingStarted) {
      return;
    }

    this.pollingStarted = true;

    const maxDurationMs = 60_000; // stop after 60s
    const startedAt = Date.now();

    const poll = async () => {
      if (!this.order) return;
      try {
        const freshOrder: any = await this.ordersService.getOrderDetailed(this.order.id).toPromise();
        if (freshOrder) {
          this.order = freshOrder as IOrder;
          if (this.order.status && this.order.status !== 'Pending') {
            clearInterval(this.pollingIntervalId);
          }
        }
      } catch (err) {
        // ignore transient errors during polling
      }

      if (Date.now() - startedAt >= maxDurationMs) {
        clearInterval(this.pollingIntervalId);
      }
    };

    // initial immediate poll, then interval
    poll();
    this.pollingIntervalId = setInterval(poll, 3000);
  }
}