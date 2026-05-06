import type { ApplyModel } from "../../models/discount-service/ApplyModel";
import type { ApplyResponseModel } from "../../models/discount-service/ApplyResponseModel";
import { api } from "../../http";
import { DISCOUNT_BASE_URL } from "../../constants/EndpointConstants";
import { DISCOUNTSERVICE_APPLY } from "../../constants/EndpointConstants";

export async function ApplyAsync(apply: ApplyModel): Promise<ApplyResponseModel> {
    const res = await api.post(`${DISCOUNT_BASE_URL}${DISCOUNTSERVICE_APPLY}`, apply);

    return res.data;
}