declare module "dojo/_base/declare" {
    let declare: any;
    export = declare;
}

declare module "dijit/_WidgetBase" {
    let _WidgetBase: any;
    export = _WidgetBase;
}

declare module "dojo/topic" {
    let _topic: any;
    export = _topic;
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
    let res: ReviewResorces;
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
