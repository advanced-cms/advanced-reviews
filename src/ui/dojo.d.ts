declare module "dojo/_base/declare" {
    let declare: any;
    export = declare;
}

declare module "dijit/_WidgetBase" {
    let _WidgetBase: any;
    export = _WidgetBase;
}

declare module "epi-cms/_ContentContextMixin" {
    let _ContentContextMixin: any;
    export = _ContentContextMixin;
}

declare module "epi-cms/ApplicationSettings" {
    let ApplicationSettings: any;
    export = ApplicationSettings;
}

declare module "epi/i18n!epi/cms/nls/reviewcomponent" {
    let res: ReviewResources;
    export = res;
}

declare module "alloy-review/advancedReviewService" {
    let advancedReviewService: any;
    export = advancedReviewService;
}

interface AdvancedReviewService {
    add(id: string, data: any): Promise<any>;
    load(): Promise<any[]>;
}

declare module "alloy-external-review/external-review-service" {
    let externalReviewService: any;
    export = externalReviewService;
}

interface ExternalReviewService {
    add(isEditable: boolean): Promise<any>;
    load(): Promise<any[]>;
    delete(token: string): Promise<any>;
    share(token: string, email: string, subject: string, message: string): Promise<any>;
}
