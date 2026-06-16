import type { ApplyModel } from "../../models/discount-service/ApplyModel";
import type { ApplyResponseModel } from "../../models/discount-service/ApplyResponseModel";
import { api_authorized } from "../../http";
import { DISCOUNT_URL } from "../../constants/EndpointConstants";

export async function ApplyAsync(apply: ApplyModel): Promise<ApplyResponseModel> {
    const res = await api_authorized.post(`${DISCOUNT_URL}/Apply`, apply);

    return res.data;
}