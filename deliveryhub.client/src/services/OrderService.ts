import { fetchBaseQuery } from "@reduxjs/toolkit/query";
import { createApi } from "@reduxjs/toolkit/query/react";
import type { OrderDto } from "../models/Orders/OrderDto";
import { OrderStatus } from "../enums/orders/OrderStatus";

export const orderApi = createApi({
    reducerPath: 'orderApi',
    baseQuery: fetchBaseQuery({
        baseUrl: 'https://localhost:7225/api/order/',
    }),
    endpoints: (build) => ({
        // getOrder: build.query<OrderDto, number>({
        //     query: (id) => ({
        //         url: `${id}`,
        //         transformResponse: (response: { data: OrderDto }) => response.data,
        //     }),
        // }),

        getOrders: build.query<OrderDto[], void>({
            query: () => ({
                url: `GetOrders`,
            }),
            // transformResponse: (response: OrderDto[]) => {
            //     // console.log(response);
                
            //     return response?.map(data => ({
            //         id: data.id,
            //         orderNumber: data.orderNumber,
            //         address: data.address,
            //         createdDate: data.createdDate,
            //         orderStatus: data.status,
            //         deliveryDate: data.deliveryDate,
            //     }));
            // }
        }),
    })
});

export const { useGetOrdersQuery } = orderApi;